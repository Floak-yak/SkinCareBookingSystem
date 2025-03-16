using System;

namespace SkinCareBookingSystem.Service.Dto
{
    public class ServicesDetailResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int? ImageId { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
    }

    public class CreateServicesDetailDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int? ImageId { get; set; }
        public int ServiceId { get; set; }
    }

    public class UpdateServicesDetailDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int? ImageId { get; set; }
        public int ServiceId { get; set; }
    }
} 