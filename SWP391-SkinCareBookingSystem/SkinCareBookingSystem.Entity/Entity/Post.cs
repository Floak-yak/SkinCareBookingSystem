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
        public PostStatus PostStatus { get; set; }

        #region Relationship
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public ICollection<Content> Contents { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Image? Image {  get; set; }
        #endregion
    }

    public enum PostStatus
    {
        Approved = 1,
        Pending = 0,
        Rejected = -1,
    }
}
