using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class User
    {
        public int Id { get; set; }
        public Role Role { get; set; }
        public string FullName { get; set; }
        public DateTime YearOfBirth { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public string VerifyToken { get; set; }
        public bool IsVerified { get; set; }

        #region Relationship
        public ICollection<TestInformation>? TestInformationHistory { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Category>? Categories { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Schedule>? Schedules { get; set; }
        #endregion
    }

    public enum Role
    {
        Manager = 1,
        Staff = 2,
        SkinTherapist = 3,
        Customer = 4,
    }
}
