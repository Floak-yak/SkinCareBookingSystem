namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class QuestionOptionUpdateDto
    {
        public int? Id { get; set; }  // Null for new options, existing ID for updates
        public string OptionText { get; set; }
        public string NextQuestionId { get; set; }
        public bool IsDeleted { get; set; } = false;  // Flag to mark options for deletion
    }
} 