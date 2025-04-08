namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class RecommendedService
    {
        public int Id { get; set; }
        public int Priority { get; set; } // Higher number means higher priority
        
        // Relationships
        public int SurveyResultId { get; set; }
        public SurveyResult SurveyResult { get; set; }
        
        public int ServiceId { get; set; }
        public SkincareService Service { get; set; }
    }
} 