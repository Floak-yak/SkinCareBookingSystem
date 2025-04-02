using System.Collections.Generic;

namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class QuestionRequestDto
    {
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }
        public bool IsActive { get; set; } = true;
        public List<QuestionOptionDto> Options { get; set; } = new List<QuestionOptionDto>();
    }
}
