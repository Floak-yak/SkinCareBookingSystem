using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.Service.Service;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("Cancel")]
        public async Task<IActionResult> CancelTransaction([FromQuery] int transactionId)
        {
            if (!await _transactionService.UpdateTransaction(transactionId, -1))
            {
                return BadRequest("Cancel fail");
            }
            return Ok("Cancel Success");
        }

        [HttpGet("Checkout")]
        public async Task<IActionResult> CheckoutTransaction([FromQuery] int transactionId)
        {
            if (!await _transactionService.UpdateTransaction(transactionId, 1))
            {
                return BadRequest("Cancel fail");
            }
            return Ok("Cancel Success");
        }
    }
}
