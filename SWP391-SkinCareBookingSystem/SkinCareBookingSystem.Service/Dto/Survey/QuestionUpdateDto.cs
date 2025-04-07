using System.Collections.Generic;

namespace SkinCareBookingSystem.Service.Dto.Survey
{
        public class QuestionUpdateDto
    {
        public int Id { get; set; }
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }
        public bool IsActive { get; set; } = true;
        public List<OptionUpdateDto> Options { get; set; } = new List<OptionUpdateDto>();
    }
} 