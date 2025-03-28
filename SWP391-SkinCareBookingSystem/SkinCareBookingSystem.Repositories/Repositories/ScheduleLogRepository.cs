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

        public async Task<bool> RemoveScheduleLog(int scheduleId)
        {
            ScheduleLog scheduleLog = await _context.ScheduleLogs.FirstOrDefaultAsync(s => s.Id == scheduleId);
            if (scheduleLog != null)
            {
                _context.ScheduleLogs.Remove(scheduleLog);
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        public void Update(ScheduleLog scheduleLog)
        {
            if (scheduleLog == null)
                return;
            _context.ScheduleLogs.Update(scheduleLog);
        }
    }
}
