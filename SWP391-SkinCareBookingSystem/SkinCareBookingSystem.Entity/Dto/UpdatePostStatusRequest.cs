using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Dto
{
    public class UpdatePostStatusRequest
    {
        public int Id { get; set; }
        public PostStatus status {  get; set; }
    }
}
