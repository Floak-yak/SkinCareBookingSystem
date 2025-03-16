using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class ServicesDetail
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        [Range(1, 180)]
        public int Duration { get; set; }

        public int? ImageId { get; set; }
        
        [ForeignKey("ImageId")]
        public Image Image { get; set; }
        
        [Required]
        public int ServiceId { get; set; }
        
        [ForeignKey("ServiceId")]
        public SkincareService SkincareService { get; set; }
    }
}
