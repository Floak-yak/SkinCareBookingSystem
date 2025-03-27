using System;
using System.Collections.Generic;

namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class UpdateSurveyDto
    {
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public bool IsResult { get; set; }
        public List<UpdateOptionDto> Options { get; set; } = new List<UpdateOptionDto>();
    }
} 