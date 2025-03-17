using Net.payOS;
using Net.payOS.Types;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Repositories.Repositories;
using SkinCareBookingSystem.Service.Dto.Product;
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
        private readonly ITransactionRepository _transactionRepository;
        private readonly PayOS _payOS;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public TransactionService(PayOS payOS, ITransactionRepository transactionRepository, IProductRepository productRepository, IUserRepository userRepository)
        {
            _transactionRepository = transactionRepository;
            _payOS = payOS;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }
        public async Task<CreatePaymentResult> CreateTransaction(Booking booking)
        {
            Transaction transaction = new()
            {
                CreatedDate = DateTime.UtcNow,
                TotalMoney = booking.TotalPrice,
                TranctionStatus = (TranctionStatus)0,
                UserId = booking.UserId,
                User = booking.User,
            };

            _transactionRepository.Create(transaction);

            if (!await _transactionRepository.SaveChange())
                throw new Exception("Create transaction fail");

            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
            ItemData item = new ItemData(booking.BookingServiceSchedules.FirstOrDefault().Service.ServiceName, 1, decimal.ToInt32(booking.TotalPrice));
            var cancelUrl = "https://localhost:7101/api/Transaction/Cancel" + transaction.Id;
            var returnUrl = "https://localhost:7101/api/Transaction/Checkout" + transaction.Id;
            List<ItemData> items = new List<ItemData>();
            items.Add(item);
            PaymentData paymentData = new PaymentData(orderCode, decimal.ToInt32(booking.TotalPrice), "Thanh toan doan hang", items, cancelUrl, returnUrl);

            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
                        
            return createPayment;
        }

        public async Task<CreatePaymentResult> CreateTransaction(CheckoutCartRequest request)
        {
            User user = await _userRepository.GetUserById(request.UserId);
            if (user == null)
            {
                throw new Exception("Invalid userId");
            }

            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
            List<ItemData> items = new List<ItemData>();
            int totalPrice = 0;
            foreach (var productInfo in request.checkoutProductInformation)
            {
                Product product = await _productRepository.GetProductById(productInfo.Id);
                if (product == null) continue;
                totalPrice += (int)product.Price * productInfo.Amount;
                items.Add(new ItemData(product.ProductName, productInfo.Amount, (int)product.Price));
            }

            Transaction transaction = new()
            {
                CreatedDate = DateTime.UtcNow,
                TotalMoney = totalPrice,
                TranctionStatus = (TranctionStatus)0,
                UserId = request.UserId,
                User = user,
            };

            _transactionRepository.Create(transaction);

            if (!await _transactionRepository.SaveChange())
                throw new Exception("Create transaction fail");

            var cancelUrl = "https://localhost:7101/api/Transaction/Cancel" + transaction.Id;
            var returnUrl = "https://localhost:7101/api/Transaction/Checkout" + transaction.Id;

            PaymentData paymentData = new PaymentData(orderCode, totalPrice, "Thanh toan doan hang", items, cancelUrl, returnUrl);

            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
                        
            return createPayment;
        }

        public async Task<bool> UpdateTransaction(int transactionId, int status)
        {
            Transaction transaction = await _transactionRepository.GetById(transactionId);
            if (transaction is null)
                return false;
            transaction.TranctionStatus = (TranctionStatus)status;
            _transactionRepository.Update(transaction);
            return await _transactionRepository.SaveChange();
        }
    }
}
