using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class SurveyResponse
    {
        public int Id { get; set; }
        public DateTime ResponseDate { get; set; } = DateTime.Now;
        
        // Foreign keys
        public int SessionId { get; set; }
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
        
        // Navigation properties
        public SurveySession Session { get; set; }
        public SurveyQuestion Question { get; set; }
        public SurveyOption Option { get; set; }
    }
} 