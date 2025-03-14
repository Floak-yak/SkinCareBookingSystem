using Microsoft.EntityFrameworkCore;
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
    public class SkincareServiceRepository : ISkincareServiceRepository
    {
        private readonly AppDbContext _context;

        public SkincareServiceRepository(AppDbContext context) 
        {
            _context = context;
        }
        public void Create(SkincareService service)
        {
            _context.SkincareServices.Add(service);
        }

        public async Task<bool> Delete(int id)
        {
            SkincareService service = await GetServiceById(id);
            if (service == null) 
                return false;
            _context.SkincareServices.Remove(service);
            return await SaveChange();
        }

        public async Task<SkincareService> GetServiceById(int serviceId)
        {
            return await _context.SkincareServices
                .Include(s => s.BookingServiceSchedules)
                .FirstOrDefaultAsync(s => s.Id == serviceId);
        }

        public async Task<SkincareService> GetServiceByname(string name)
        {
            return await _context.SkincareServices
                .Include(s => s.BookingServiceSchedules)
                .FirstOrDefaultAsync(s => s.ServiceName.Equals(name));
        }

        public async Task<List<SkincareService>> GetServices()
        {
            return await _context.SkincareServices
                .Include(s => s.BookingServiceSchedules)
                .OrderBy(s => s.ServiceName)
                .ToListAsync();
        }

        public async Task<bool> SaveChange()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<SkincareService>> Search(string search)
        {
            return await _context.SkincareServices
                .Where(s => s.ServiceName.Contains(search))
                .ToListAsync();
        }

        public void Update(SkincareService service)
        {
            _context.SkincareServices.Update(service);
        }
        public async Task<bool> IsServiceExist(string name)
        {
            return await GetServiceByname(name) != null;
        }

        public async Task<Dictionary<int, List<SkincareService>>> GetRandomServicesByCategory(int count)
        {
            var services = await _context.SkincareServices
                .Include(s => s.Category)
                .Include(s => s.Image)
                .GroupBy(s => s.CategoryId)
                .OrderBy(g => g.First().Category.CategoryName)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.OrderBy(r => Guid.NewGuid()).Take(count).ToList()
                );

            return services;
        }
    }
}
