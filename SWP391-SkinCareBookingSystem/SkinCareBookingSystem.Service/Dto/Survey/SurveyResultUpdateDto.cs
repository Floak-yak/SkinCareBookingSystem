using System.Collections.Generic;

namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class SurveyResultUpdateDto
    {
        public int Id { get; set; }
        public string ResultId { get; set; }
        public string ResultText { get; set; }
        public string SkinType { get; set; }
        public string RecommendationText { get; set; }
        public List<RecommendedServiceUpdateDto> RecommendedServices { get; set; } = new List<RecommendedServiceUpdateDto>();
    }
} 