using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.BookingDto
{
    public class GetPaybackCancelBookingsResponse
    {
        public int BookingId { get; set; }
        public string FullName { get; set; }
        public DateTime YearOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentNumber { get; set; }
        public DateTime date { get; set; }
        public Decimal TotalAmount { get; set; }
        public Image? Image { get; set; }
    }
}
