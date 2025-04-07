using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.BookingDto
{
    public class GetCancelBookingByUserIdResponse
    {
        public DateTime CreatedTime { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }
        public string ServiceName { get; set; }
        public bool Status { get; set; }
    }
}
