using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime DatePost { get; set; }
        public string ImageLink { get; set; }
        public bool IsApproved { get; set; }

        #region Relationship
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public ICollection<Content> Contents { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        #endregion
    }
}
