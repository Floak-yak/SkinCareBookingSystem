using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Dto
{
    public class CreatePostWithContentsRequest
    {
        public string Title { get; set; }
        public string imageLink { get; set; }
        public List<CreatePostContentRequest> contents { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
    }
}
