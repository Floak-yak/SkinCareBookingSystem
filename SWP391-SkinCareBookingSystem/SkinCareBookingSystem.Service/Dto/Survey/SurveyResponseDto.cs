using System;
using System.Collections.Generic;

namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class SurveyResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
} 