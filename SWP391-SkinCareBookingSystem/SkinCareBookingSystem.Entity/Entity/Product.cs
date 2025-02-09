using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Decimal Price { get; set; }

        #region Relationship
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public Image Image { get; set; }
        #endregion
    }
}
