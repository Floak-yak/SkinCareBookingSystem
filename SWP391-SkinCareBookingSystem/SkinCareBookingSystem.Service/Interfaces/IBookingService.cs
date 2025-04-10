﻿using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Dto.BookingDto;
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
        public Task<List<GetBookingsResponse>> GetBookingsAsync();
        public Task<List<GetPaybackCancelBookingsResponse>> GetPaidCancelBookings();
        public Task<List<GetPaybackCancelBookingsResponse>> GetPayBackCancelBookings();
        public Task<List<GetCancelBookingByUserIdResponse>> GetCancelBookingByUserId(int userId);        
        public Task<bool> CompletePayment(int bookingId);
        public Task<bool> DeleteBooking(int bookingId);
        public Task<Booking> UpdateBooking(int bookingId, string serviceName);
        public Task<Booking> CreateBooking(CreateBookingRequest request);
        public Task<bool> UpdateBookingDate(int bookingId, int userId, DateTime newDate);
        public Task<bool> CancelBooking(int bookingId, int userId);
        public Task<User> RandomSkinTherapist(List<User> listUser);
        public Task<bool> UpdateBookingDateTime(UpdateBookingRequest request);
        public Task<bool> SkinTherapistCheckout(int skinTherapistId, int scheduleLogId);
        public Task<bool> UpdateBookingTherapist(UpdateBookingTherapistRequest request);
        public Task<bool> UserCheckin(int userId);
    }
}
