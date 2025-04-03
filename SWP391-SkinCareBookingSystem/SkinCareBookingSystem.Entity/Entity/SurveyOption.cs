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
        public int Points { get; set; }
        public string SkinTypeId { get; set; }
        
        // Relationships
        public int QuestionId { get; set; }
        public SurveyQuestion Question { get; set; }
        public ICollection<SurveyResponse> Responses { get; set; }
    }
} 