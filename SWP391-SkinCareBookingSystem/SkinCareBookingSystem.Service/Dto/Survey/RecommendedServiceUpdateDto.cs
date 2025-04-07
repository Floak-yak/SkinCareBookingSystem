namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class RecommendedServiceUpdateDto
    {
        public int? Id { get; set; }
        public int ServiceId { get; set; }
        public int Priority { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
} 