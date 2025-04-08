namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class SkinTypePointsDto
    {
        public int? Id { get; set; }
        public string SkinTypeId { get; set; }
        public int Points { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
} 