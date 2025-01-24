using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        
        #region Relationship
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<Product> Products { get; set; }
        #endregion
    }
}
