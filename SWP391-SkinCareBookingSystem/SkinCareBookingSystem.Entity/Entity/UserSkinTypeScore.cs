namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class UserSkinTypeScore
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public string SkinTypeId { get; set; }
        public int Score { get; set; }
        
        public SurveySession Session { get; set; }
    }
} 