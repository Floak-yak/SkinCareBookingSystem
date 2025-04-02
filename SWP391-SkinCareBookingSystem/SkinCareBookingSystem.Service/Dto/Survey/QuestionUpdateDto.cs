using System.Collections.Generic;

namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class QuestionUpdateDto
    {
        public int Id { get; set; }
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }
        public bool IsActive { get; set; }
        public List<QuestionOptionUpdateDto> Options { get; set; } = new List<QuestionOptionUpdateDto>();
    }
} 