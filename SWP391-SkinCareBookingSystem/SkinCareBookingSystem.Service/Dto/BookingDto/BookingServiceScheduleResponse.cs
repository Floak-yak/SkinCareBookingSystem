using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.BookingDto
{
    public class BookingServiceScheduleResponse
    {
        public int ServiceId { get; set; }
        public int BookingId { get; set; }
        public int ScheduleLogId { get; set; }
        public BusinessObject.Entity.ScheduleLog ScheduleLog { get; set; }
    }
}
