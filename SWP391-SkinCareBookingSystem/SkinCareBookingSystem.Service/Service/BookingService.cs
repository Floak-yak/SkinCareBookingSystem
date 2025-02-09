using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Service
{
    public class BookingService : IBookingService
    {
        public Task<bool> CreateBooking(DateTime CreatedTime, DateTime Date)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteBooking(int bookingId)
        {
            throw new NotImplementedException();
        }

        public Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Booking>> GetBookingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateBooking(Booking booking)
        {
            throw new NotImplementedException();
        }
    }
}
