using System.Collections.Generic;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class SurveyOption
    {
        public int Id { get; set; }
        public string OptionText { get; set; }
        
        // Relationships
        public int QuestionId { get; set; }
        public SurveyQuestion Question { get; set; }
        public ICollection<SurveyResponse> Responses { get; set; }
        public ICollection<OptionSkinTypePoints> SkinTypePoints { get; set; } = new List<OptionSkinTypePoints>();
    }
} 