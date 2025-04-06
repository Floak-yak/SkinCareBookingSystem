using System;
using System.Collections.Generic;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class SurveySession
    {
        public int Id { get; set; }
        public DateTime CompletedDate { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; }
        
        public int? UserId { get; set; }
        public User User { get; set; }
        
        public int? SurveyResultId { get; set; }
        public SurveyResult SurveyResult { get; set; }
        
        public ICollection<SurveyResponse> Responses { get; set; }
        public ICollection<UserSkinTypeScore> SkinTypeScores { get; set; }
        
        public string SelectedQuestionId { get; set; }
    }
} 