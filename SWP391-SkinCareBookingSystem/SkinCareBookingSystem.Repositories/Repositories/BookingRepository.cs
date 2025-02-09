﻿using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Data;
using SkinCareBookingSystem.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext appDbContext) => _context = appDbContext;
        public void CreateBooking(Booking booking) =>
            _context.Bookings.Add(booking);

        public async Task<bool> DeleteBooking(int bookingId)
        {
            Booking booking = await GetBookingByIdAsync(bookingId);
            if (booking is null)
                return false;
            _context.Bookings.Remove(booking);
            return await SaveChange();
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId) =>
            await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);

        public async Task<List<Booking>> GetBookingsAsync() =>
            await _context.Bookings
            .Include(b => b.BookingServiceSchedules)
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedTime)
            .ToListAsync();

        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId) =>
            await _context.Bookings
            .Include(b => b.BookingServiceSchedules)
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedTime)
            .Where(b => b.UserId == userId)
            .ToListAsync();

        public async Task<bool> SaveChange() =>
            await _context.SaveChangesAsync() > 0;

        public void UpdateBooking(Booking booking) =>
            _context.Bookings.Update(booking);
    }
}
