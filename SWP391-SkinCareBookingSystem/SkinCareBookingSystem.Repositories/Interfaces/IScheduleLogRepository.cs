using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Interfaces
{
    public interface IScheduleLogRepository
    {
        public Task<ScheduleLog> GetScheduleLogById(int scheduleId); 
        public Task<bool> RemoveScheduleLog(int scheduleId);
    }
}
