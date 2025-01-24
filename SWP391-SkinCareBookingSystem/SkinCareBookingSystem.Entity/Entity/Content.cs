using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class Content
    {
        public int Id { get; set; }
        public string ContentOfPost { get; set; }
        public ContentType ContentType { get; set; }
        public int StackPosition { get; set; }
        public string ImageLink { get; set; }
        
        #region Relationship
        public int PostId { get; set; }
        public Post Post { get; set; }
        #endregion
    }

    public enum ContentType
    {
        ContentOfImage = 1,
        Summary = 2,
    }
}
