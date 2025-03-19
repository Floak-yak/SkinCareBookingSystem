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
    public class ScheduleLogRepository : IScheduleLogRepository
    {
        private readonly AppDbContext _context;

        public ScheduleLogRepository(AppDbContext context) => _context = context;
        public async Task<ScheduleLog> GetScheduleLogById(int scheduleId)
        {
            return await _context.ScheduleLogs.Include(sl => sl.Schedule).FirstOrDefaultAsync(sl => sl.Id == scheduleId);
        }
    }
}
