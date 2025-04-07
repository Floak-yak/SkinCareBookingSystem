using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        public Task<Booking> GetBookingByIdAsync(int bookingId);
        public Task<Booking> GetBookingByScheduleLogId(int scheduleLogId);  
        public Task<List<Booking>> GetBookingsByUserIdAsync(int userId);
        public Task<List<Booking>> GetCancelBookings();
        public Task<List<Booking>> GetCancelBookingByUserId(int userId);
        public Task<List<Booking>> GetBookingsAsync();
        public Task<bool> DeleteBooking(int bookingId);
        public Task<bool> DeleteBooking(Booking booking);
        public void UpdateBooking(Booking booking);
        public void UpdateBooking(List<Booking> bookings);
        public void CreateBooking(Booking booking);
        public Task<bool> SaveChange();
        public Task<User> GetRandomSkinTherapistAsync();
    }
}
