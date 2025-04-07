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
                .Include(u => u.Posts)
                .Include(u => u.Category)
                .Include(u => u.Bookings)
                .Include(u => u.Schedules)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<User> GetUserById(int userId)
        {
            return await _context.Users                
                .Include(u => u.Posts)
                .Include(u => u.Category)
                .Include(u => u.Bookings)
                .Include(u => u.Schedules)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUserByName(string userName)
        {
            return await _context.Users                
                .Include(u => u.Posts)
                .Include(u => u.Category)
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
            if (_context.Attach(user).State == EntityState.Modified)
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

        public async Task<List<User>> GetSkinTherapistsFreeInTimeSpan(DateTime dateTime, int Duration, int categoryId)
        {
            DateTime endDate = dateTime.AddMinutes(Duration);
            List<User> users = await _context.Users.Where(u => u.CategoryId == categoryId).ToListAsync();
            List<User> removeUser = new List<User>();
            if (users.Count == 0)
                return null;
            foreach (var user in users)
            {
                List<Schedule> schedules = await _context.Schedules
                    .Include(s => s.ScheduleLogs)
                    .Where(s => s.UserId == user.Id).ToListAsync();
                if (schedules is null) continue;
                //So sanh cung ngay, cung gio 
                Schedule schedule = schedules.FirstOrDefault(s => s.DateWork.Date.ToShortDateString()
                        .Equals(dateTime.Date.ToShortDateString()));
                if (schedule is null) continue;
                if (schedule.ScheduleLogs.FirstOrDefault(sl => sl.TimeStartShift.Hour == dateTime.Hour && sl.TimeStartShift.Minute == dateTime.Minute && !sl.IsCancel) != null)
                {
                    removeUser.Add(user);
                    continue;
                }                        
                else if (schedule.ScheduleLogs
                        .FirstOrDefault(sl => 
                        sl.TimeStartShift.Hour*60 
                        + sl.TimeStartShift.Minute 
                        + Duration 
                        - dateTime.Hour*60 
                        - sl.TimeStartShift.Minute > 0
                        && sl.TimeStartShift.Hour < dateTime.Hour
                        && !sl.IsCancel) != null)
                {
                    removeUser.Add(user);
                    continue;
                }else if (schedule.ScheduleLogs
                        .FirstOrDefault(sl => 
                        - sl.TimeStartShift.Hour * 60
                        - sl.TimeStartShift.Minute
                        + Duration
                        + dateTime.Hour * 60
                        + sl.TimeStartShift.Minute > 0
                        && sl.TimeStartShift.Hour > dateTime.Hour
                        && !sl.IsCancel) != null)
                {
                    removeUser.Add(user);
                    continue;
                }
            }
            foreach (var user in removeUser)
            {
                users.Remove(user);
            }
            return users;
        }

        public async Task<List<User>> GetSkinTherapistsByCategoryId(int categoryId) =>
            await _context.Users
                
                .Include(u => u.Posts)
                .Include(u => u.Category)
                .Include(u => u.Bookings)
                .Include(u => u.Schedules)
                .OrderBy(u => u.FullName)
            .Where(u => u.CategoryId == categoryId).ToListAsync();

        public async Task<User> GetSkinTherapistById(int SkinTherapistId) =>
            await _context.Users                
                .Include(u => u.Posts)
                .Include(u => u.Category)
                .Include(u => u.Bookings)
                .Include(u => u.Schedules).FirstOrDefaultAsync(s => s.Id == SkinTherapistId && s.Role == Role.SkinTherapist);
    }
}