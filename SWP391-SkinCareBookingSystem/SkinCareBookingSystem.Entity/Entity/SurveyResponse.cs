using System;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class SurveyResponse
    {
        public int Id { get; set; }
        public DateTime ResponseDate { get; set; } = DateTime.Now;
        public string SkinTypeId { get; set; }
        
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