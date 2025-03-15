using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.User
{
    public class UploadAvatarForUserRequest
    {
        public int UserId { get; set; }
        public int ImageId { get; set; }
    }
}
