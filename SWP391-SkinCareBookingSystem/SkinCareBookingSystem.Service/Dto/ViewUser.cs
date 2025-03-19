using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto
{
    public class ViewUser
    {
        public int Id { get; set; }
        public Role Role { get; set; }
        public string FullName { get; set; }
        public DateTime YearOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsVerified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
