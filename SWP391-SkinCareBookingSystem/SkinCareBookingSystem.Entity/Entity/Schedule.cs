using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class Schedule
    {
        public int Id { get; set; }
        public DateTime DateWork { get; set; }

        #region Relationship
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<ScheduleLog> ScheduleLogs { get; set; }
        #endregion
    }
}
