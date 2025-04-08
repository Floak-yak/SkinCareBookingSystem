using AutoMapper;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.BusinessObject.Helper;
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
                    WorkingTime = skincareService.WorkTime,
                    IsCancel = false,
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
                      
            BookingServiceSchedule bookingService = new()
            {
                ServiceId = skincareService.Id,
                Service = skincareService,
                ScheduleLog = schedule.ScheduleLogs.FirstOrDefault(sl => sl.TimeStartShift == date),
                ScheduleLogId = schedule.ScheduleLogs.FirstOrDefault(sl => sl.TimeStartShift == date).ScheduleId
            };

            Booking booking = new()
            {
                Status = (BookingStatus)0,
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
                throw new Exception("Create booking fail");
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

            if (booking.Status == BookingStatus.Checkin || booking.Status == BookingStatus.Completed)
                return false;

            booking.Status = BookingStatus.Cancel;

            Transaction transaction = await _transactionRepository.GetTransactionByABookingIdAndUserId(userId, bookingId);
            if (transaction is null) return false;
            if (transaction.TranctionStatus is TranctionStatus.Paid)
            {
                transaction.TranctionStatus = TranctionStatus.WattingForPayBack;
            }

            _transactionRepository.Update(transaction);
            _bookingRepository.UpdateBooking(booking);

            ScheduleLog scheduleLog = await _scheduleLogRepository.GetScheduleLogById(booking.BookingServiceSchedules.FirstOrDefault().ScheduleLogId);

            if (scheduleLog is null)
                throw new Exception("ScheduleLog not found");

            scheduleLog.IsCancel = true;

            _scheduleLogRepository.Update(scheduleLog);

            return await _bookingRepository.SaveChange();
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

        public async Task<bool> SkinTherapistCheckout(int skinTherapistId, int scheduleLogId)
        {
            User skinTherapist = await _userRepository.GetSkinTherapistById(skinTherapistId);
            if (skinTherapist is null)
                throw new InvalidOperationException("Invalid skintherapistId");
            ScheduleLog scheduleLog = await _scheduleLogRepository.GetScheduleLogById(scheduleLogId);
            if (scheduleLog is null)
                throw new InvalidOperationException("Invalid scheduleLogId");
            if (scheduleLog.TimeStartShift.Hour > DateTime.Now.Hour)
                throw new InvalidOperationException("Can not checkout before the shift start time!!!");
            if (scheduleLog.TimeStartShift.Hour == DateTime.Now.Hour && scheduleLog.TimeStartShift.Minute > DateTime.Now.Minute)
                throw new InvalidOperationException("Can not checkout before the shift start time!!!");
            Booking booking = await _bookingRepository.GetBookingByScheduleLogId(scheduleLogId);
            if (scheduleLog is null)
                throw new Exception("Booking not found");
            booking.Status = BookingStatus.Completed;
            _bookingRepository.UpdateBooking(booking);
            return await _bookingRepository.SaveChange();
        }

        public async Task<bool> UpdateBookingTherapist(UpdateBookingTherapistRequest request)
        {
            try
            {
                Booking booking = await _bookingRepository.GetBookingByIdAsync(request.BookingId);
                if (booking == null) 
                    throw new InvalidOperationException("Booking ID not found");

                User newTherapist = await _userRepository.GetSkinTherapistById(request.NewSkinTherapistId);
                if (newTherapist == null || !newTherapist.IsVerified || newTherapist.Role != Role.SkinTherapist)
                    throw new InvalidOperationException("Invalid therapist ID");

                var serviceSchedule = booking.BookingServiceSchedules?.FirstOrDefault();
                if (serviceSchedule == null) 
                    throw new InvalidOperationException("No booking service schedule found");

                SkincareService service = await _skincareServiceRepository.GetServiceById(serviceSchedule.ServiceId);
                if (service == null) 
                    throw new InvalidOperationException("Service not found");

                if (service.CategoryId != newTherapist.CategoryId) 
                    throw new InvalidOperationException("Therapist's category not match");

                ScheduleLog currentScheduleLog = await _scheduleLogRepository.GetScheduleLogById(serviceSchedule.ScheduleLogId);
                if (currentScheduleLog == null) throw new InvalidOperationException("Current schedule log not found");

                if (currentScheduleLog.Schedule.UserId == request.NewSkinTherapistId) return true;

                DateTime shiftTime = currentScheduleLog.TimeStartShift;
                
                List<User> freeTherapists = await _userRepository.GetSkinTherapistsFreeInTimeSpan(
                    shiftTime, 
                    service.WorkTime, 
                    service.CategoryId);

                if (freeTherapists == null || !freeTherapists.Any(t => t.Id == request.NewSkinTherapistId))
                    throw new InvalidOperationException("Therapist is not available");

                currentScheduleLog.IsCancel = true;
                _scheduleLogRepository.Update(currentScheduleLog);

                if (newTherapist.Schedules == null)
                    newTherapist.Schedules = new List<Schedule>();

                Schedule newTherapistSchedule = newTherapist.Schedules.FirstOrDefault(s => s.DateWork.Date == shiftTime.Date);
                if (newTherapistSchedule == null)
                {
                    newTherapistSchedule = new Schedule
                    {
                        DateWork = shiftTime.Date,
                        User = newTherapist,
                        UserId = newTherapist.Id,
                        ScheduleLogs = new List<ScheduleLog>()
                    };
                    newTherapist.Schedules.Add(newTherapistSchedule);
                }

                ScheduleLog newScheduleLog = new ScheduleLog
                {
                    TimeStartShift = shiftTime,
                    WorkingTime = service.WorkTime,
                    Schedule = newTherapistSchedule,
                    ScheduleId = newTherapistSchedule.Id
                };

                if (newTherapistSchedule.ScheduleLogs == null)
                    newTherapistSchedule.ScheduleLogs = new List<ScheduleLog>();

                newTherapistSchedule.ScheduleLogs.Add(newScheduleLog);

                serviceSchedule.ScheduleLog = newScheduleLog;
                serviceSchedule.ScheduleLogId = newScheduleLog.Id;

                _bookingRepository.UpdateBooking(booking);
                return await _bookingRepository.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UserCheckin(int userId)
        {
            User user = await _userRepository.GetUserById(userId);
            if (user is null)
                throw new InvalidOperationException("Invalid userId");
            List<Booking> bookings = user.Bookings.Where(b => b.Date.Day == DateTime.Now.Day).ToList();
            foreach (var booking in bookings)
            {
                booking.Status = BookingStatus.Checkin;
            }
            _bookingRepository.UpdateBooking(bookings);
            return await _bookingRepository.SaveChange();
        }

        public async Task<List<GetPaybackCancelBookingsResponse>> GetPaidCancelBookings()
        {
            List<GetPaybackCancelBookingsResponse> responses = new();
            try
            {
                List<Booking> bookings = await _bookingRepository.GetCancelBookings();
                List<int> bookingIdList = new();
                foreach (var booking in bookings)
                {
                    bookingIdList.Add(booking.Id);
                }
                List<Transaction> transactions = await _transactionRepository.GetTransactionsByBookingId(bookingIdList);
                foreach (Transaction transaction in transactions)
                {
                    Booking booking = bookings.FirstOrDefault(b => b.Id == transaction.BookingId);
                    if (booking is null)
                    {
                        throw new Exception("Invalid data of booking");
                    }
                    if (transaction.TranctionStatus != TranctionStatus.PaidBack)
                        bookings.Remove(booking);
                    else
                    {
                        if (booking.User.PaymentMethod != null && booking.User.PaymentNumber != null)
                        {
                            responses.Add(new GetPaybackCancelBookingsResponse()
                            {
                                date = booking.Date,
                                Email = booking.User.Email,
                                FullName = booking.User.FullName,
                                PaymentMethod = booking.User.PaymentMethod,
                                PaymentNumber = EncryptionHelper.Decrypt(booking.User.PaymentNumber),
                                PhoneNumber = booking.User.PhoneNumber,
                                TotalAmount = transaction.TotalMoney,
                                YearOfBirth = booking.User.YearOfBirth
                            });
                        }
                        else
                            responses.Add(new GetPaybackCancelBookingsResponse()
                            {
                                date = booking.Date,
                                Email = booking.User.Email,
                                FullName = booking.User.FullName,
                                PhoneNumber = booking.User.PhoneNumber,
                                TotalAmount = transaction.TotalMoney,
                                YearOfBirth = booking.User.YearOfBirth
                            });
                        bookings.Remove(booking);
                    }                        
                }                
            }
            catch
            {
                throw new Exception("Invalid data of booking");
            }            
            return responses;
        }

        public async Task<List<GetPaybackCancelBookingsResponse>> GetPayBackCancelBookings()
        {
            List<GetPaybackCancelBookingsResponse> responses = new();
            try
            {
                List<Booking> bookings = await _bookingRepository.GetCancelBookings();
                List<int> bookingIdList = new();
                foreach (var booking in bookings)
                {
                    bookingIdList.Add(booking.Id);
                }
                List<Transaction> transactions = await _transactionRepository.GetTransactionsByBookingId(bookingIdList);
                foreach (Transaction transaction in transactions)
                {
                    Booking booking = bookings.FirstOrDefault(b => b.Id == transaction.BookingId);
                    if (booking is null)
                    {
                        throw new Exception("Invalid data of booking");
                    }
                    if (transaction.TranctionStatus != TranctionStatus.WattingForPayBack)
                        bookings.Remove(booking);
                    else
                    {
                        if (booking.User.PaymentMethod != null && booking.User.PaymentNumber != null)
                        {
                            responses.Add(new GetPaybackCancelBookingsResponse()
                            {
                                date = booking.Date,
                                Email = booking.User.Email,
                                FullName = booking.User.FullName,
                                PaymentMethod = booking.User.PaymentMethod,
                                PaymentNumber = EncryptionHelper.Decrypt(booking.User.PaymentNumber),
                                PhoneNumber = booking.User.PhoneNumber,
                                TotalAmount = transaction.TotalMoney,
                                YearOfBirth = booking.User.YearOfBirth
                            });
                        }
                        else
                            responses.Add(new GetPaybackCancelBookingsResponse()
                            {
                                date = booking.Date,
                                Email = booking.User.Email,
                                FullName = booking.User.FullName,
                                PhoneNumber = booking.User.PhoneNumber,
                                TotalAmount = transaction.TotalMoney,
                                YearOfBirth = booking.User.YearOfBirth
                            });
                        bookings.Remove(booking);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return responses;
        }

        public async Task<List<GetCancelBookingByUserIdResponse>> GetCancelBookingByUserId(int userId)
        {
            List<GetCancelBookingByUserIdResponse> responses = new();
            try
            {
                List<Booking> bookings = await _bookingRepository.GetCancelBookingByUserId(userId);
                List<int> bookingIdList = new();
                foreach (var booking in bookings)
                {
                    bookingIdList.Add(booking.Id);
                }
                List<Transaction> transactions = await _transactionRepository.GetTransactionsByBookingId(bookingIdList);
                foreach (Transaction transaction in transactions)
                {
                    Booking booking = bookings.FirstOrDefault(b => b.Id == transaction.BookingId);
                    if (booking is null)
                    {
                        throw new Exception("Invalid data of booking");
                    }
                    if (transaction.TranctionStatus != TranctionStatus.WattingForPayBack && transaction.TranctionStatus != TranctionStatus.PaidBack)
                        bookings.Remove(booking);
                    else
                    {
                        responses.Add(new GetCancelBookingByUserIdResponse()
                        {
                            CreatedTime = booking.CreatedTime,
                            Date = booking.Date,
                            ServiceName = booking.BookingServiceSchedules.First().Service.ServiceName,
                            TotalPrice = booking.TotalPrice,
                            Status = transaction.TranctionStatus == TranctionStatus.PaidBack
                        });
                        bookings.Remove(booking);
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            return responses;
        }

        public async Task<bool> CompletePayment(int bookingId)
        {
            Booking booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking is null)
                throw new InvalidOperationException("Invalid bookingId");
            Transaction transaction = await _transactionRepository.GetTransactionByBookingId(bookingId);
            if (transaction is null)
                throw new Exception("Invalid data of booking");
            if (transaction.Image is null)
                return false;
            transaction.TranctionStatus = TranctionStatus.PaidBack;
            _transactionRepository.Update(transaction);
            return await _transactionRepository.SaveChange();
        }
    }
}
