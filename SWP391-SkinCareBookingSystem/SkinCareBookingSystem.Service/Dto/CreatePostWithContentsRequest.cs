using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto
{
    public class CreatePostWithContentsRequest
    {
        public string Title { get; set; }
        public string imageLink { get; set; }
        public string summary { get; set; }
        public string content { get; set; } //Html
        public int CategoryId { get; set; }
        public int UserId { get; set; }
    }
}
