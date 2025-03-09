using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Data;
using SkinCareBookingSystem.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsers()
        {
            return await _context.Users
                .Include(u => u.TestInformationHistory)
                .Include(u => u.Posts)
                .Include(u => u.Categories)
                .Include(u => u.Bookings)
                .Include(u => u.Schedules)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<User> GetUserById(int userId)
        {
            return await _context.Users
                .Include(u => u.TestInformationHistory)
                .Include(u => u.Posts)
                .Include(u => u.Categories)
                .Include(u => u.Bookings)
                .Include(u => u.Schedules)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUserByName(string userName)
        {
            return await _context.Users
                .Include(u => u.TestInformationHistory)
                .Include(u => u.Posts)
                .Include(u => u.Categories)
                .Include(u => u.Bookings)
                .Include(u => u.Schedules)
                .FirstOrDefaultAsync(u => u.FullName.Equals(userName));
        }

        public void Create(User user)
        {
            _context.Users.Add(user);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public async Task<bool> Delete(int userId)
        {
            var user = await GetUserById(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            return await SaveChange();
        }

        public async Task<bool> SaveChange()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsUserExist(string userName)
        {
            return await _context.Users.AnyAsync(u => u.FullName.Equals(userName));
        }

        public async Task<User> GetUserByEmail(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));

        public async Task<User> GetUserByNameByEmailByRole(string email, Role role, string userName) =>
            await _context.Users
                .Where(u => u.Email.Equals(email) && u.Role.Equals(role) 
                    && u.FullName.Equals(userName))
            .SingleOrDefaultAsync();

        public async Task<List<User>> GetStaffs() =>
            await _context.Users.Where(u => u.IsVerified && u.Role == Role.Staff).ToListAsync();

        public async Task<List<User>> GetCustomers() =>
            await _context.Users.Where(u => u.IsVerified && u.Role == Role.Customer).ToListAsync();

        public async Task<List<User>> GetSkinTherapists() =>
            await _context.Users.Where(u => u.IsVerified && u.Role == Role.SkinTherapist).ToListAsync();
    }
}