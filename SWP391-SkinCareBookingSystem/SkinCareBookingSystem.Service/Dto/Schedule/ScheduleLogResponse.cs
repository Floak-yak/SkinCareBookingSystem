using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto.BookingDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.Schedule
{
    public class ScheduleLogResponse
    {
        public int Id { get; set; }
        public int WorkingTime { get; set; }
        public DateTime TimeStartShift { get; set; }
        public string ServiceName { get; set; }
        public ICollection<BookingServiceScheduleResponse> BookingServiceSchedules { get; set; }
    }
}
