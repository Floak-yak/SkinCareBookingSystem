using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Net.payOS;
using SkinCareBookingSystem.Service.Dto.Transaction;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.Service.Service;
using SkinCareBookingSystem.BusinessObject.Entity;
using Transaction = SkinCareBookingSystem.BusinessObject.Entity.Transaction;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;
        private readonly PayOS _payOS;

        public TransactionController(ITransactionService transactionService, IMapper mapper, PayOS payOS)
        {
            _mapper = mapper;
            _transactionService = transactionService;
            _payOS = payOS;
        }

        [HttpPut("Cancel")]
        public async Task<IActionResult> CancelTransaction([FromQuery] int transactionId)
        {
            if (!await _transactionService.UpdateTransaction(transactionId, -1))
            {
                return BadRequest("Cancel fail");
            }
            return Ok("Cancel Success");
        }

        [HttpPut("Checkout")]
        public async Task<IActionResult> CheckoutTransaction([FromQuery] int transactionId)
        {
            if (!await _transactionService.UpdateTransaction(transactionId, 1))
            {
                return BadRequest("Checkout fail");
            }
            return Ok("Checkout Success");
        }

        [HttpGet("GetTransactionByUserId")]
        public async Task<IActionResult> GetTransactionByUserId([FromQuery] int userId)
        {
            return Ok(await _transactionService.GetTransactionByUserId(userId));
        }

        [HttpGet("Gets")]
        public async Task<IActionResult> GetAllTransactions()
        {
            return Ok(await _transactionService.GetAllTransactions());
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetPayment([FromRoute] int orderId)
        {
            PaymentLinkInformation paymentLinkInformation = await _payOS.getPaymentLinkInformation(orderId);
            if (paymentLinkInformation is null)
            {
                throw new ArgumentNullException("Invalid id - " + nameof(paymentLinkInformation));
            }
            return Ok(paymentLinkInformation.status);
        }
        [HttpGet("GetTransactionByBookingId")]
        public async Task<IActionResult> GetTransactionByBookingId([FromQuery] int bookingId)
        {
            if (bookingId <= 0)
                throw new InvalidOperationException("Invaid Id");
            Transaction transaction = await _transactionService.GetTransactionByBookingId(bookingId);
            if (transaction is null)
                throw new InvalidOperationException("Invaid Id");
            return Ok(transaction);
        }
        [HttpGet("GetTransactionById")]
        public async Task<IActionResult> GetTransactionById([FromQuery] int transactionId)
        {
            if (transactionId <= 0)
                throw new InvalidOperationException("Invaid Id");
            Transaction transaction = await _transactionService.GetTransactionById(transactionId);
            if (transaction is null)
                throw new InvalidOperationException("Invaid Id");
            return Ok(transaction);
        }
    }
}
