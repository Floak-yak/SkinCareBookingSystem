using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto
{
    public class GetProductResponse
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Decimal Price { get; set; }
        public Image Image { get; set; }
    }
}
