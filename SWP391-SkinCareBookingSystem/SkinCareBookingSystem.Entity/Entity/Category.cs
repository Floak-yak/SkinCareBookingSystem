﻿using System;
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
        public ICollection<Post> Posts { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<SkincareService> skincareServices { get; set; }
        #endregion
    }
}
