using System;

namespace SkinCareBookingSystem.Service.Dto.Survey
{
    public class UpdateOptionDto
    {
        public string Id { get; set; }
        public string OptionText { get; set; }
        public string NextQuestionId { get; set; }
    }
} 