using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto
{
    public class CreatePostContentRequest
    {
        public string ContentOfPost { get; set; }
        public int Position { get; set; }
        public string imageLink { get; set; }
        public int PostId { get; set; }
    }
}
