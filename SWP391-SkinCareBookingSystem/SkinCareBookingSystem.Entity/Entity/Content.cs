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
        public int Position { get; set; }
        
        #region Relationship
        public Image? Image { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        #endregion
    }
}
