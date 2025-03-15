using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.BookingDto
{
    public class CreateBookingRequest
    {
        public string Date {  get; set; } 
        public string Time { get; set; }
        public string ServiceName {  get; set; } 
        public int UserId { get; set; }
        public int SkinTherapistId {  get; set; }
        public int CategoryId { get; set; }
    }
}
