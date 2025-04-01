using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.ScheduleLog
{
    public class ScheduleLogDto
    {
        public int Id { get; set; }
        public int WorkingTime { get; set; }
        public DateTime TimeStartShift { get; set; }
        public bool IsCancel { get; set; }
        public int ScheduleId { get; set; }
    }
}
