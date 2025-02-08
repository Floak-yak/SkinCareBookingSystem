using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime Date {  get; set; }
        public Decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }

        #region Relationship
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<BookingServiceSchedule> BookingServiceSchedules { get; set; }
        #endregion
    }

    public enum BookingStatus
    {
        Pending = 1,
        Paid = 2,
    }
}
