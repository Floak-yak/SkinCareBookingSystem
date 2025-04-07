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
        public async Task<IActionResult> Gets()
        {
            List<GetBookingsResponse> responses = await _bookingService.GetBookingsAsync();
            if (responses is null)
            {
                return NotFound();
            }
            return Ok(responses);
        }

        [HttpGet("GetPayBackCancelBookings")]
        public async Task<IActionResult> GetPayBackCancelBookings()
        {
            var responses = await _bookingService.GetPayBackCancelBookings();
            if (responses is null)
            {
                return NotFound();
            }
            return Ok(responses);
        }

        [HttpGet("GetPaidCancelBookings")]
        public async Task<IActionResult> GetPaidCancelBookings()
        {
            var responses = await _bookingService.GetPaidCancelBookings();
            if (responses is null)
            {
                return NotFound();
            }
            return Ok(responses);
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage([FromQuery] int bookingId)
        {
            List<GetBookingsResponse> responses = await _bookingService.GetBookingsAsync();
            if (responses is null)
            {
                return NotFound();
            }
            return Ok(responses);
        }

        [HttpPut("CompletePayment")]
        public async Task<IActionResult> CompletePayment([FromQuery] int bookingId)
        {
            var responses = await _bookingService.CompletePayment(bookingId);
            if (responses)
            {
                return BadRequest("Complete fail");
            }
            return Ok("Complete success");
        }

        [HttpGet("GetPayBackCancelBookingByUserId")]
        public async Task<IActionResult> GetPayBackCancelBookingByUserId([FromQuery] int userId)
        {
            var responses = await _bookingService.GetCancelBookingByUserId(userId);
            if (responses is null)
            {
                return NotFound();
            }
            return Ok(responses);
        }

        [HttpPut("SkinTherapistCheckout")]
        public async Task<IActionResult> SkinTherapistCheckout([FromQuery] int skinTherapistId, int scheduleLogId)
        {
            if (skinTherapistId <= 0)
                return BadRequest("Invalid skinTherapistId");
            if (scheduleLogId <= 0)
                return BadRequest("Invalid scheduleLogId");
            bool result;
            try
            {
                result = await _bookingService.SkinTherapistCheckout(skinTherapistId, scheduleLogId);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            if (!result)
            {
                return BadRequest("Checkout fail");
            }
            return Ok("Checkout success");
        }

        [HttpPut("SkinTherapistCheckin")]
        public async Task<IActionResult> SkinTherapistCheckin([FromQuery] int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid userId");
            bool result;
            try
            {
                result = await _bookingService.UserCheckin(userId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            if (!result)
            {
                return BadRequest("Checkout fail");
            }
            return Ok("Checkout success");
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
            return Ok(await _transactionService.CreateTransaction(booking));
        }

        [HttpPut("UpdateDate")]
        public async Task<IActionResult> UpdateBookingDate(int bookingId, int userId, DateTime newDate)
        {
            bool result = await _bookingService.UpdateBookingDate(bookingId, userId, newDate);
            if (!result)
                return BadRequest("Update booking date failed");
            return Ok("Booking date updated successfully");
        }
        
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateBooking([FromBody] UpdateBookingRequest request)
        {
            if (string.IsNullOrEmpty(request.Date) || string.IsNullOrEmpty(request.Time))
                return BadRequest("Date and time are required");

            try
            {
                if (!await _bookingService.UpdateBookingDateTime(request))
                    return BadRequest("Update booking failed");
                return Ok("Booking updated successfully");
            }
            catch (FormatException)
            {
                return BadRequest("Invalid date or time format");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateTherapist")]
        public async Task<IActionResult> UpdateBookingTherapist([FromBody] UpdateBookingTherapistRequest request)
        {
            if (request.BookingId <= 0)
                return BadRequest("Invalid booking ID");

            if (request.NewSkinTherapistId <= 0)
                return BadRequest("Invalid skin therapist ID");

            try
            {
                var result = await _bookingService.UpdateBookingTherapist(request);
                if (!result)
                    return BadRequest("Update booking failed");

                return Ok("Booking updated successfully");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Cancel")]
        public async Task<IActionResult> CancelBooking(int bookingId, int userId)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                if (booking == null)
                {
                    return NotFound(new { success = false, message = $"Booking with ID {bookingId} not found" });
                }

                if (booking.UserId != userId)
                {
                    return BadRequest(new { success = false, message = "You are not authorized to cancel this booking" });
                }

                if (booking.Status == BookingStatus.Completed)
                {
                    return BadRequest(new { success = false, message = "Cannot cancel a completed booking" });
                }

                if (booking.Status == BookingStatus.Waitting)
                {
                    return BadRequest(new { success = false, message = "Cannot cancel a paid booking" });
                }

                TimeSpan timeSinceCreation = DateTime.UtcNow - booking.CreatedTime;
                if (timeSinceCreation.TotalDays > 1)
                {
                    return BadRequest(new { success = false, message = "Booking can only be cancelled within 24 hours of creation" });
                }

                bool result = await _bookingService.CancelBooking(bookingId, userId);
                if (!result)
                {
                    return StatusCode(500, new { success = false, message = "Failed to cancel booking due to a server error" });
                }

                return Ok(new { success = true, message = "Booking cancelled successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred while cancelling the booking", error = ex.Message });
            }
        }
    }
}
