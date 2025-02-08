using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class Service
    {
        public int Id { get; set; }
        public string ServiceName { get; set; }

        #region Relationship
        public Decimal Price { get; set; }
        public DateTime WorkTime { get; set; }
        public ICollection<BookingServiceSchedule> BookingServiceSchedules { get; set; }
        #endregion
    }
}
