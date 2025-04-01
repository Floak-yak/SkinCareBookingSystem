using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class SurveyOption
    {
        public int Id { get; set; }
        public string OptionText { get; set; }
        public string NextQuestionId { get; set; } // ID of the next question or RESULT_X for results
        
        // Relationships
        public int QuestionId { get; set; }
        public SurveyQuestion Question { get; set; }
        public ICollection<SurveyResponse> Responses { get; set; }
    }
} 