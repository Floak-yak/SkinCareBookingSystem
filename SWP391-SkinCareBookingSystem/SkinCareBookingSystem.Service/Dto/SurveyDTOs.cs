using System;
using System.Collections.Generic;

namespace SkinCareBookingSystem.Service.Dto
{
    public class NextQuestionRequest
    {
        public string CurrentQuestionId { get; set; }
        public string SelectedOptionId { get; set; }
    }
} 