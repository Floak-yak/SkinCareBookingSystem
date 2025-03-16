using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.Product
{
    public class CheckoutCartRequest
    {
        public int UserId { get; set; }
        public List<CheckoutProductInformation> checkoutProductInformation {  get; set; }
    }
}
