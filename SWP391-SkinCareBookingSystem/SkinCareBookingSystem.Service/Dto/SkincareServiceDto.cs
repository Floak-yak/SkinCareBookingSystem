using System;
using System.ComponentModel.DataAnnotations;

namespace SkinCareBookingSystem.Service.Dto
{
    public class SkincareServiceCreateDTO
    {
        [Required]
        [StringLength(100)]
        public string ServiceName { get; set; }

        [Required]
        [StringLength(500)]
        public string ServiceDescription { get; set; }

        public string ImageLink { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }

        [Required]
        public DateTime WorkTime { get; set; }
    }

    public class SkincareServiceUpdateDTO
    {
        [StringLength(100)]
        public string ServiceName { get; set; }

        [StringLength(500)]
        public string ServiceDescription { get; set; }

        public string ImageLink { get; set; }

        [Range(1, int.MaxValue)]
        public int? CategoryId { get; set; }

        [Range(0.01, 1000000)]
        public decimal? Price { get; set; }

        public DateTime? WorkTime { get; set; }
    }
} 