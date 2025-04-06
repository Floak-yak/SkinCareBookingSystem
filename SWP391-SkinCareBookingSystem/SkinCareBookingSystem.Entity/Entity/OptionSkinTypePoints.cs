namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class OptionSkinTypePoints
    {
        public int Id { get; set; }
        public int OptionId { get; set; }
        public string SkinTypeId { get; set; }
        public int Points { get; set; }
        
        // Relationship
        public SurveyOption Option { get; set; }
    }
} 