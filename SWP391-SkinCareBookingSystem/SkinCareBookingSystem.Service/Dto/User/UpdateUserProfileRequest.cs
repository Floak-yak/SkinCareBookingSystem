using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.User
{
    public class UpdateUserProfileRequest
    {
        public int userId {  get; set; }
        public string FullName { get; set; }
        public DateTime YearOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentNumber { get; set; }
        public IFormFile? Image { get; set; }
    }
}
