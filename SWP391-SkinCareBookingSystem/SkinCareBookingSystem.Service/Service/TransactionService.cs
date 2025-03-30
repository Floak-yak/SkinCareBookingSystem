using AutoMapper;
using Net.payOS;
using Net.payOS.Types;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Repositories.Repositories;
using SkinCareBookingSystem.Service.Dto.Product;
using SkinCareBookingSystem.Service.Dto.Transaction;
using SkinCareBookingSystem.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transaction = SkinCareBookingSystem.BusinessObject.Entity.Transaction;

namespace SkinCareBookingSystem.Service.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;
        private readonly PayOS _payOS;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public TransactionService(PayOS payOS, ITransactionRepository transactionRepository, IProductRepository productRepository, IUserRepository userRepository, IMapper mapper, IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            _transactionRepository = transactionRepository;
            _payOS = payOS;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }
        public async Task<CreateTransactionResponse> CreateTransaction(Booking booking)
        {            
            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
            ItemData item = new ItemData(booking.BookingServiceSchedules.FirstOrDefault().Service.ServiceName, 1, decimal.ToInt32(booking.TotalPrice));
            var cancelUrl = "";
            var returnUrl = "";
            List<ItemData> items = new List<ItemData>();
            items.Add(item);
            PaymentData paymentData = new PaymentData(orderCode, decimal.ToInt32(booking.TotalPrice), "Thanh toan doan hang", items, cancelUrl, returnUrl);

            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
            if (createPayment is null)
            {
                await _bookingRepository.DeleteBooking(booking);
                throw new Exception("Something wrong when create transaction");
            }
                
            Transaction transaction = new()
            {
                CreatedDate = DateTime.UtcNow,
                TotalMoney = booking.TotalPrice,
                TranctionStatus = (TranctionStatus)0,
                UserId = booking.UserId,
                User = booking.User,
                BookingId = booking.Id,
                OrderCode = createPayment.orderCode,
                QrCode = createPayment.qrCode,
            };

            _transactionRepository.Create(transaction);

            if (!await _transactionRepository.SaveChange())
                throw new Exception("Create transaction fail");

            return new CreateTransactionResponse() { createPaymentResult = createPayment, transactionId = transaction.Id };
        }

        public async Task<CreateTransactionResponse> CreateTransaction(CheckoutCartRequest request)
        {
            User user = await _userRepository.GetUserById(request.UserId);
            if (user == null)
            {
                throw new Exception("Invalid userId");
            }

            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
            List<ItemData> items = new List<ItemData>();
            int totalPrice = 0;
            List<Product> products = new List<Product>();
            foreach (var productInfo in request.checkoutProductInformation)
            {
                Product product = await _productRepository.GetProductById(productInfo.Id);
                if (product == null) continue;
                totalPrice += (int)product.Price * productInfo.Amount;
                items.Add(new ItemData(product.ProductName, productInfo.Amount, (int)product.Price));
                products.Add(product);
            }
            if (products.Count == 0)
            {
                throw new Exception("Invalid Product");
            }

            var cancelUrl = $"";
            var returnUrl = $"";

            PaymentData paymentData = new PaymentData(orderCode, totalPrice, "Thanh toan doan hang", items, cancelUrl, returnUrl);

            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

            if (createPayment is null)
            {
                throw new Exception("Something wrong when create transaction");
            }

            Transaction transaction = new()
            {
                CreatedDate = DateTime.UtcNow,
                TotalMoney = totalPrice,
                TranctionStatus = (TranctionStatus)0,
                UserId = request.UserId,
                User = user,
                Products = products,
                OrderCode = createPayment.orderCode,
                QrCode = createPayment.qrCode,
            };

            _transactionRepository.Create(transaction);

            if (!await _transactionRepository.SaveChange())
                throw new Exception("Create transaction fail");

            return new CreateTransactionResponse() { createPaymentResult = createPayment, transactionId = transaction.Id };
        }

        public async Task<List<GetTransactionResponse>> GetAllTransactions()
        {
            List<Transaction> transactions = await _transactionRepository.GetAllTransactions();
            List<GetTransactionResponse> responses = new List<GetTransactionResponse>();
            foreach (Transaction transaction in transactions)
            {
                GetTransactionResponse transactionResponse = _mapper.Map<GetTransactionResponse>(transaction);
                if (transaction.BookingId != null)
                {
                    transactionResponse.Booking = await _bookingRepository.GetBookingByIdAsync((int)transaction.BookingId);
                    transactionResponse.BookingType = "Booking's transaction";
                }else
                    transactionResponse.BookingType = "Order's transaction";

                responses.Add(transactionResponse);
            }
            return responses;
        }

        public async Task<Transaction> GetTransactionByBookingId(int bookingId) =>
            await _transactionRepository.GetTransactionByBookingId(bookingId);

        public async Task<Transaction> GetTransactionById(int transactionId) =>
            await _transactionRepository.GetById(transactionId);

        public async Task<List<GetTransactionResponse>> GetTransactionByUserId(int userId)
        {
            List<Transaction> transactions = await _transactionRepository.GetByUserId(userId);
            List<GetTransactionResponse> responses = new List<GetTransactionResponse>();
            foreach (Transaction transaction in transactions)
            {
                GetTransactionResponse transactionResponse = _mapper.Map<GetTransactionResponse>(transaction);
                if (transaction.BookingId != null)
                {
                    transactionResponse.Booking = await _bookingRepository.GetBookingByIdAsync((int)transaction.BookingId);
                    transactionResponse.BookingType = "Booking's transaction";
                }
                else
                    transactionResponse.BookingType = "Order's transaction";
                responses.Add(transactionResponse);
            }
            return responses;
        }

        public async Task<bool> UpdateTransaction(int transactionId, int status)
        {
            Transaction transaction = await _transactionRepository.GetById(transactionId);
            if (transaction is null)
                return false;
            if (transaction.BookingId is not null)
            {
                Booking booking = await _bookingRepository.GetBookingByIdAsync((int)transaction.BookingId);
                if (booking is null)
                    return false;
                if (status == 1)
                {
                    booking.Status = BookingStatus.Waitting;
                }
                else
                {
                    await _payOS.cancelPaymentLink(transaction.OrderCode);
                    booking.Status = (BookingStatus)status;
                }
            }                          

            transaction.TranctionStatus = (TranctionStatus)status;
            _transactionRepository.Update(transaction);
            return await _transactionRepository.SaveChange();
        }
    }
}
