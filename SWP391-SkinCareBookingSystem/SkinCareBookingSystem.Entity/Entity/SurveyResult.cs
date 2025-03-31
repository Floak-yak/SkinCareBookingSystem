using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.BusinessObject.Entity
{
    public class SurveyResult
    {
        public int Id { get; set; }
        public string ResultId { get; set; }
        public string ResultText { get; set; }
        public string SkinType { get; set; }
        public string RecommendationText { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        // Relationships
        public ICollection<SurveySession> Sessions { get; set; }
        public ICollection<RecommendedService> RecommendedServices { get; set; }
    }
} 