namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class RecommendedServiceUpdateDto
    {
        public int? Id { get; set; }  // Null for new services, existing ID for updates
        public int ServiceId { get; set; }
        public int Priority { get; set; }
        public bool IsDeleted { get; set; } = false;  // Flag to mark services for deletion
    }
} 