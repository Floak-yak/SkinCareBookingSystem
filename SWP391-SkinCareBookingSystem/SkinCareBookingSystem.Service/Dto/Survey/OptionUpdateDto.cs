namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class OptionUpdateDto
    {
        public int? Id { get; set; }
        public string OptionText { get; set; }
        public int Points { get; set; } = 0;
        public bool IsDeleted { get; set; } = false;
        public List<SkinTypePointsDto> SkinTypePoints { get; set; } = new List<SkinTypePointsDto>();
    }
} 