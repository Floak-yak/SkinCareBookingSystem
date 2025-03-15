using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class ScheduleLog
    {
        public int Id { get; set; }
        public int WorkingTime { get; set; }
        public DateTime TimeStartShift { get; set; }

        #region Relationship
        public int ScheduleId { get; set; }
        public Schedule Schedule { get; set; }
        public ICollection<BookingServiceSchedule> BookingServiceSchedules { get; set; }
        #endregion
    }
}
