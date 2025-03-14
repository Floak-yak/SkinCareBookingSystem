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
        public decimal Price { get; set; }

        [Required]
        [Range(1, 90, ErrorMessage = "Work time must be between 1 and 90 minutes")]
        public int WorkTime { get; set; }
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

        public decimal? Price { get; set; }

        [Range(1, 90, ErrorMessage = "Work time must be between 1 and 90 minutes")]
        public int? WorkTime { get; set; }
    }
} 