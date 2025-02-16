using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.Service.Service;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService, IMapper mapper, ITransactionService transactionService)
        {
            _transactionService = transactionService;
            _mapper = mapper;
            _bookingService = bookingService;
        }

        [HttpGet("Gets")]
        public async Task<List<GetBookingsResponse>> Gets()
        {
            var result = _mapper.Map<List<GetBookingsResponse>>(await _bookingService.GetBookingsAsync());
            return result;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateBooking(DateTime Date, string serviceName, int userId)
        {
            Booking booking = await _bookingService.CreateBooking(Date, serviceName, userId);
            if (booking is null)
                return BadRequest("Create fail");
            return Ok(_transactionService.CreateTransaction(booking));
        }
    }
}
