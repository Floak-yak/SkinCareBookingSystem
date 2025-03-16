using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Dto.BookingDto;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.Service.Service;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
        {
            Booking booking;
            try
            {
                booking = await _bookingService.CreateBooking(request);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            if (booking is null)
                return BadRequest("Create fail");
            return Ok(_transactionService.CreateTransaction(booking));
        }

        [HttpPut("UpdateDate")]
        public async Task<IActionResult> UpdateBookingDate(int bookingId, int userId, DateTime newDate)
        {
            bool result = await _bookingService.UpdateBookingDate(bookingId, userId, newDate);
            if (!result)
                return BadRequest("Update booking date failed");
            return Ok("Booking date updated successfully");
        }

        [HttpDelete("Cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId, int userId)
        {
            bool result = await _bookingService.CancelBooking(bookingId, userId);
            if (!result)
                return BadRequest("Cancel booking failed.");
            return Ok("Booking cancelled successfully");
        }
    }
}
