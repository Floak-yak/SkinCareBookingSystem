using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.Transaction
{
    public class GetTransactionResponse
    {
        public int Id { get; set; }
        public Decimal TotalMoney { get; set; }
        public DateTime CreatedDate { get; set; }
        public TranctionStatus TranctionStatus { get; set; }
        public int UserId { get; set; }
        public ViewUser User { get; set; }
        public Booking Booking { get; set; }
        public List<GetProductResponse>? Products { get; set; }
        public string BookingType { get; set; }
    }
}
