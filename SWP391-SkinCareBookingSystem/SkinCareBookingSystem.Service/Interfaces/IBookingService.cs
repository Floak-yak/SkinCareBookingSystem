using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface IBookingService
    {
        public Task<Booking> GetBookingByIdAsync(int bookingId);
        public Task<List<Booking>> GetBookingsByUserIdAsync(int userId);
        public Task<List<Booking>> GetBookingsAsync();
        public Task<bool> DeleteBooking(int bookingId);
        public Task<bool> UpdateBooking(Booking booking);
        public Task<bool> CreateBooking(DateTime CreatedTime, DateTime Date);
    }
}
