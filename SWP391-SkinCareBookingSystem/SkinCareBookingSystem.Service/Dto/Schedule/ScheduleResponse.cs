using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.Schedule
{
    public class ScheduleResponse
    {
        public DateTime DateWork { get; set; }

        public ICollection<ScheduleLogResponse>? ScheduleLogs { get; set; }
    }
}
