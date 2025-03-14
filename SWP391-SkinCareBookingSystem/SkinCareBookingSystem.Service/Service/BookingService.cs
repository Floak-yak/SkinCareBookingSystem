﻿using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Repositories.Repositories;
using SkinCareBookingSystem.Service.Dto.BookingDto;
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
        public async Task<Booking> CreateBooking(CreateBookingRequest request)
        {
            SkincareService skincareService = await _skincareServiceRepository
                .GetServiceByname(request.ServiceName);
            if (skincareService is null)
                return null;

            User user = await _userRepository.GetUserById(request.UserId);
            if (user is null)
                return null;

            User skintherapist = await _userRepository.GetUserById(request.UserId);
            if (!skintherapist.IsVerified || skintherapist.Role != (Role)3)
                return null;

            TimeOnly time = TimeOnly.Parse(request.Time.ToString());
            DateTime date = DateTime.Parse(request.Date);
            date.AddHours(time.Hour);
            date.AddHours(time.Minute);

            if (skintherapist is null)
                skintherapist = await RandomSkinTherapist(await _userRepository.GetSkinTherapistsFreeInTimeSpan(date, skincareService.WorkTime, request.CategoryId));

            if (skintherapist is null)
                return null;

            if (skintherapist.Schedules is null)
            {
                skintherapist.Schedules = new List<Schedule>();                 
            }

            if (skintherapist.Schedules.FirstOrDefault(s => s.DateWork == date) is null)
            {   
                Schedule scheduleCreate = new Schedule()
                {
                    DateWork = date,
                    User = skintherapist,
                    UserId = skintherapist.Id,
                };
                skintherapist.Schedules.Add(scheduleCreate);
            }

            Schedule schedule = skintherapist.Schedules.FirstOrDefault(s => s.DateWork == date);
            if (schedule is null)
            {
                return null;
            }

            if (schedule.ScheduleLogs.FirstOrDefault(sl => sl.TimeStartShift == date) == null)
            {
                ScheduleLog scheduleLog = new ScheduleLog()
                {
                    TimeStartShift = date,
                    WorkingTime = skincareService.WorkTime
                };
                schedule.ScheduleLogs.Add(scheduleLog);
            }

            if (!await _userRepository.SaveChange())
                return null;
                      
            BookingServiceSchedule bookingService = new()
            {
                ServiceId = skincareService.Id,
                Service = skincareService,
                ScheduleLog = schedule.ScheduleLogs.FirstOrDefault(sl => sl.TimeStartShift == date),
                ScheduleLogId = schedule.ScheduleLogs.FirstOrDefault(sl => sl.TimeStartShift == date).ScheduleId
            };

            Booking booking = new()
            {
                Status = (BookingStatus)1,
                CreatedTime = DateTime.UtcNow,
                Date = date,
                TotalPrice = skincareService.Price,
                User = user,
                UserId = request.UserId,
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

        public async Task<bool> UpdateBookingDate(int bookingId, int userId, DateTime newDate)
        {
            Booking booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking is null || booking.UserId != userId)
                return false;

            TimeSpan tDiff = newDate - booking.Date;
            if (Math.Abs(tDiff.TotalDays) > 7)
                return false;

            booking.Date = newDate;
            _bookingRepository.UpdateBooking(booking);
            return await _bookingRepository.SaveChange();
        }

        public async Task<bool> CancelBooking(int bookingId, int userId)
        {
            Booking booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking is null || booking.UserId != userId)
                return false;

            TimeSpan tDiff = DateTime.UtcNow - booking.CreatedTime;
            if (tDiff.TotalDays > 1)
                return false;

            if (booking.Status == BookingStatus.Paid)
                return false;

            return await _bookingRepository.DeleteBooking(bookingId);
        }

        public async Task<User> RandomSkinTherapist(List<User> listUser)
        {
            if (listUser.Count == 0)
                return null;
            return listUser[new Random().Next(0, listUser.Count)];
        }
    }
}
