using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.BookingDto
{
    public class UpdateBookingTherapistRequest
    {
        public int BookingId { get; set; }
        public int NewSkinTherapistId { get; set; }
    }
} 