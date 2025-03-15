using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.User
{
    public class UpdateUserDescriptionRequest
    {
        public int UserId { get; set; } 
        public string Description { get; set; }
    }
}
