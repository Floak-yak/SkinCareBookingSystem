using System;

namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class NextQuestionRequest
    {
        public string CurrentQuestionId { get; set; }
        public string SelectedOptionId { get; set; }
    }
} 