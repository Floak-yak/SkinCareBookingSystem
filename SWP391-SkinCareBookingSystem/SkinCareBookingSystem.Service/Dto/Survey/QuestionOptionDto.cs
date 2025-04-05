namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class QuestionOptionDto
    {
        public string OptionText { get; set; }
        public int Points { get; set; }
        public List<SkinTypePointsDto> SkinTypePoints { get; set; } = new List<SkinTypePointsDto>();
    }
} 