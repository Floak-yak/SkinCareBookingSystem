﻿using AutoMapper;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Repositories.Repositories;
using SkinCareBookingSystem.Service.Dto;
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
        private readonly ITransactionRepository _transactionRepository;
        private readonly IScheduleLogRepository _scheduleLogRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ISkincareServiceRepository _skincareServiceRepository;

        public BookingService(IBookingRepository bookingRepository, ISkincareServiceRepository skincareServiceRepository, IUserRepository userRepository, IMapper mapper, IScheduleLogRepository scheduleLogRepository, ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
            _scheduleLogRepository = scheduleLogRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
            _skincareServiceRepository = skincareServiceRepository;
        }
        public async Task<Booking> CreateBooking(CreateBookingRequest request)
        {
            SkincareService skincareService = await _skincareServiceRepository
                .GetServiceByname(request.ServiceName);
            if (skincareService is null)
                throw new InvalidOperationException(nameof(request.ServiceName));

            if (skincareService.CategoryId != request.CategoryId)
                throw new InvalidOperationException("Category is not match with category of service" + nameof(request.CategoryId));

            User user = await _userRepository.GetUserById(request.UserId);
            if (user is null)
                throw new InvalidOperationException(nameof(request.UserId));

            User skintherapist = await _userRepository.GetUserById(request.SkinTherapistId);
            if (request.SkinTherapistId != 0)
                if (!skintherapist.IsVerified || skintherapist.Role != (Role)3 || skintherapist.CategoryId is null)
                    throw new InvalidOperationException("Invalid: " + nameof(skincareService));

            TimeOnly time = TimeOnly.Parse(request.Time.ToString());
            DateTime dateRequest = DateTime.Parse(request.Date);
            DateTime date = new DateTime(dateRequest.Date.Year, dateRequest.Date.Month, dateRequest.Date.Day, time.Hour, time.Minute, 0);
            

             if (skintherapist is null)
                  skintherapist = await RandomSkinTherapist(await _userRepository.GetSkinTherapistsFreeInTimeSpan(date, skincareService.WorkTime, request.CategoryId));
            else
            {
                List<User> listSkintherrpist = await _userRepository.GetSkinTherapistsFreeInTimeSpan(date, skincareService.WorkTime, request.CategoryId);
                if (listSkintherrpist is not null)
                {
                    if (!listSkintherrpist.Contains(skintherapist))
                        throw new ArgumentNullException(nameof(skintherapist), "This skintherapist is busy");
                }             
            }

            if (skintherapist is null)
                throw new ArgumentNullException(nameof(skintherapist), "No free skintherapist in this time");

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

            //Check category of skintherapist with skinservice. Avoid trouble from value
            if (skincareService.CategoryId != skintherapist.CategoryId)
                throw new InvalidOperationException("Category is not match with category of service" + nameof(request.CategoryId));

            Schedule schedule = skintherapist.Schedules.FirstOrDefault(s => s.DateWork == date);
            if (schedule is null)
            {
                return null;
            }

            if (schedule.ScheduleLogs is null)
            {
                schedule.ScheduleLogs = new List<ScheduleLog>();
                ScheduleLog scheduleLog = new ScheduleLog()
                {
                    TimeStartShift = date,
                    WorkingTime = skincareService.WorkTime
                };
                schedule.ScheduleLogs.Add(scheduleLog);
            }
            else if (schedule.ScheduleLogs.FirstOrDefault(sl => sl.TimeStartShift == date) == null)
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
            if (booking.BookingServiceSchedules is null)
            {
                booking.BookingServiceSchedules = new List<BookingServiceSchedule>();
            }
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

        public async Task<List<GetBookingsResponse>> GetBookingsAsync()
        {
            List<Booking> bookings = await _bookingRepository.GetBookingsAsync();
            if (bookings is null)
                return null;
            List<GetBookingsResponse> result = new();
            foreach (Booking booking in bookings)
            {
                SkincareService service = await _skincareServiceRepository
                    .GetServiceById(booking.BookingServiceSchedules
                    .FirstOrDefault(b => b.BookingId == booking.Id).ServiceId);
                if (service is null)
                    continue;
                ScheduleLog log = await _scheduleLogRepository.GetScheduleLogById(booking.BookingServiceSchedules
                    .FirstOrDefault(b => b.BookingId == booking.Id).ScheduleLogId);
                if (log is null)
                    continue;
                User skintherapist = await _userRepository
                    .GetUserById(log.Schedule.UserId);
                GetBookingsResponse response = new()
                {
                    Id = booking.Id,
                    CreatedTime = booking.CreatedTime,
                    Date = booking.Date,
                    Status = booking.Status,
                    TotalPrice = booking.TotalPrice,
                    User = _mapper.Map<ViewUser>(booking.User),
                    ServiceName = service.ServiceName,
                    SkintherapistName = skintherapist.FullName,
                    CategoryId = service.CategoryId,
                };
                result.Add(response);
            }
            return result;
        }

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

            //TimeSpan tDiff = DateTime.UtcNow - booking.CreatedTime;
            //if (tDiff.TotalDays > 1)
            //    return false;

            Transaction transaction = await _transactionRepository.GetTransactionByABookingIdAndUserId(userId, bookingId);
            if (transaction is null) return false;
            if (transaction.TranctionStatus is TranctionStatus.Paid)
            {
                return false;
            }

            if (booking.Status == BookingStatus.Paid)
                return false;

            return await _bookingRepository.DeleteBooking(bookingId);
        }

        public async Task<bool> UpdateBookingDateTime(UpdateBookingRequest request)
        {
            Booking booking = await _bookingRepository.GetBookingByIdAsync(request.BookingId);
            if (booking is null)
                return false;

            User therapist = await _userRepository.GetUserById(request.SkinTherapistId);
            if (therapist == null || !therapist.IsVerified || therapist.Role != (Role)3)
                return false;

            TimeOnly time = TimeOnly.Parse(request.Time);
            DateTime date = DateTime.Parse(request.Date);
            date = date.AddHours(time.Hour).AddMinutes(time.Minute);

            booking.Date = date;
            
            var bookingServiceSchedule = booking.BookingServiceSchedules.FirstOrDefault();
            if (bookingServiceSchedule == null)
                throw new ArgumentNullException("BookingServiceSchedule is null");

            if (bookingServiceSchedule != null && bookingServiceSchedule.ScheduleLog != null)
            {
                bookingServiceSchedule.ScheduleLog.TimeStartShift = date;
                bookingServiceSchedule.ScheduleLog.Schedule.UserId = request.SkinTherapistId;
            }

            _bookingRepository.UpdateBooking(booking);
            return await _bookingRepository.SaveChange();
        }

        public async Task<User> RandomSkinTherapist(List<User> listUser)
        {
            if (listUser is null)
                return null;
            if (listUser.Count == 0)
                return null;
            return listUser[new Random().Next(0, listUser.Count)];
        }
    }
}
