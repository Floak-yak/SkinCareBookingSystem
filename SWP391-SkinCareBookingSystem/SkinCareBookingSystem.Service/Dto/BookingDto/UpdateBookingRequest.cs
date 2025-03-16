using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.BookingDto
{
    public class UpdateBookingRequest
    {
        public int BookingId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public int SkinTherapistId { get; set; }
    }
} 