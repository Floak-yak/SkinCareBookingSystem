using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class QuestionRequestDto
    {
        public string QuestionId { get; set; }
        public string QuestionText { get; set; }
        public bool IsActive { get; set; } = true;
        public List<QuestionOptionDto> Options { get; set; } = new List<QuestionOptionDto>();
    }

    public class QuestionOptionDto
    {
        public string OptionText { get; set; }
        public string NextQuestionId { get; set; }
    }
}
