using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class BookingServiceSchedule
    {
        public int ServiceId { get; set; }
        public int BookingId { get; set; }
        public int ScheduleLogId { get; set; }
    
        public Service Service { get; set; }
        public Booking Booking { get; set; }
        public ScheduleLog ScheduleLog { get; set; }
    }
}
