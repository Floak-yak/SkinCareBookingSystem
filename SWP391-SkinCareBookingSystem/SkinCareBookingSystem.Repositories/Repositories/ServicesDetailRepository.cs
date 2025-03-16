using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Data;
using SkinCareBookingSystem.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Repositories
{
    public class ServicesDetailRepository : IServicesDetailRepository
    {
        private readonly AppDbContext _context;

        public ServicesDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServicesDetail>> GetAllServicesDetailsAsync()
        {
            return await _context.ServicesDetails
                .Include(sd => sd.SkincareService)
                .Include(sd => sd.Image)
                .ToListAsync();
        }

        public async Task<ServicesDetail> GetServicesDetailByIdAsync(int id)
        {
            return await _context.ServicesDetails
                .Include(sd => sd.SkincareService)
                .Include(sd => sd.Image)
                .FirstOrDefaultAsync(sd => sd.Id == id);
        }

        public async Task<ServicesDetail> CreateServicesDetailAsync(ServicesDetail servicesDetail)
        {
            await _context.ServicesDetails.AddAsync(servicesDetail);
            await _context.SaveChangesAsync();
            return servicesDetail;
        }

        public async Task<ServicesDetail> UpdateServicesDetailAsync(ServicesDetail servicesDetail)
        {
            _context.Entry(servicesDetail).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return servicesDetail;
        }

        public async Task DeleteServicesDetailAsync(int id)
        {
            var servicesDetail = await _context.ServicesDetails.FindAsync(id);
            if (servicesDetail != null)
            {
                _context.ServicesDetails.Remove(servicesDetail);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ServicesDetail>> GetServicesDetailsByServiceIdAsync(int serviceId)
        {
            return await _context.ServicesDetails
                .Include(sd => sd.SkincareService)
                .Include(sd => sd.Image)
                .Where(sd => sd.ServiceId == serviceId)
                .ToListAsync();
        }
    }
}