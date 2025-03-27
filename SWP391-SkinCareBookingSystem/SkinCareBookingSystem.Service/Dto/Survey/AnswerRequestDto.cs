using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class AnswerRequestDto
    {
        public int SessionId { get; set; }
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
    }
}