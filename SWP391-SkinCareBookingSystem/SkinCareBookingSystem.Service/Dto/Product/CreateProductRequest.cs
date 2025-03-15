using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.Product
{
	public class CreateProductRequest
	{
		public string ProductName { get; set; }
		public DateTime CreatedDate { get; set; }
		public Decimal Price { get; set; }
		public int CategoryId { get; set; }
		public int ImageId { get; set; }
	}
}