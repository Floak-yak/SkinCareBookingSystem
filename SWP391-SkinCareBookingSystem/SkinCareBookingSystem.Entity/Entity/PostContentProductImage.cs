using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
	public class PostContentProductImage
	{
		public int ImageId { get; set; }
		public Image Image { get; set; }
		public int ProductId { get; set; }
		public Product Product { get; set; }	
		public int ContentId { get; set; }
		public Content Content { get; set; }
		public int PostId { get; set; }
		public Post Post { get; set; }
	}
}
