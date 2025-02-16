using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Repositories.Repositories;
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
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ISkincareServiceRepository _skincareServiceRepository;

        public BookingService(IBookingRepository bookingRepository, ISkincareServiceRepository skincareServiceRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
            _skincareServiceRepository = skincareServiceRepository;
        }
        public async Task<Booking> CreateBooking(DateTime Date, string serviceName, int userId)
        {
            SkincareService skincareService = await _skincareServiceRepository
                .GetServiceByname(serviceName);
            if (skincareService is null)
                return null;

            User user = await _userRepository.GetUserById(userId);
            if (user is null)
                return null;

            BookingServiceSchedule bookingService = new()
            {
                ServiceId = userId,
                Service = skincareService,
            };

            Booking booking = new()
            {
                Status = (BookingStatus)1,
                CreatedTime = DateTime.UtcNow,
                Date = Date,
                TotalPrice = skincareService.Price,
                User = user,
                UserId = userId,
            };
            booking.BookingServiceSchedules.Add(bookingService);

            _bookingRepository.CreateBooking(booking);
            if (!await _bookingRepository.SaveChange())
            {
                return null;
            }

            return booking;
        }

        public async Task<bool> DeleteBooking(int bookingId) =>
            await _bookingRepository.DeleteBooking(bookingId);

        public async Task<Booking> GetBookingByIdAsync(int bookingId) =>
            await _bookingRepository.GetBookingByIdAsync(bookingId);

        public async Task<List<Booking>> GetBookingsAsync() =>
            await _bookingRepository.GetBookingsAsync();

        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId) =>
            await _bookingRepository.GetBookingsByUserIdAsync(userId);

        public async Task<Booking> UpdateBooking(int bookingId, string serviceName)
        {
            Booking booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking is null)
                return null;
            SkincareService skincareService = await _skincareServiceRepository
                .GetServiceByname(serviceName);
            if (skincareService is null)
                return null;
            booking.TotalPrice = skincareService.Price;
            booking.BookingServiceSchedules
                .FirstOrDefault(b => b.Service != null).Service = skincareService;
            booking.BookingServiceSchedules
                .FirstOrDefault(b => b.Service != null).ServiceId = skincareService.Id;
            _bookingRepository.UpdateBooking(booking);
            if (!await _bookingRepository.SaveChange())
                return null;
            return booking;
        }
    }
}
