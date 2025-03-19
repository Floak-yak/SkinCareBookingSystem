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
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<SkincareService> skincareServices { get; set; } = new List<SkincareService>();
        #endregion
    }
}
