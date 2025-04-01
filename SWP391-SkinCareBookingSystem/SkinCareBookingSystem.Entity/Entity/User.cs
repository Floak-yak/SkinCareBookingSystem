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
        public string? VerifyToken { get; set; }
        public bool IsVerified { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; }

        #region Relationship
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Schedule>? Schedules { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
        public Image? Image { get; set; }
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
