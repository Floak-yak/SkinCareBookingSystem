using System;
using System.Collections.Generic;

namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class CreateSurveyDto
    {
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public bool IsResult { get; set; }
        public List<CreateOptionDto> Options { get; set; } = new List<CreateOptionDto>();
    }
} 