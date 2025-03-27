using System;
using System.Collections.Generic;

namespace SkinCareBookingSystem.Service.Dto
{
    public class NextQuestionRequest
    {
        public string CurrentQuestionId { get; set; }
        public string SelectedOptionId { get; set; }
    }

    public class CreateSurveyDto
    {
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public bool IsResult { get; set; }
        public List<CreateOptionDto> Options { get; set; } = new List<CreateOptionDto>();
    }

    public class CreateOptionDto
    {
        public string OptionText { get; set; }
        public string NextQuestionId { get; set; }
    }

    public class UpdateSurveyDto
    {
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public bool IsResult { get; set; }
        public List<UpdateOptionDto> Options { get; set; } = new List<UpdateOptionDto>();
    }

    public class UpdateOptionDto
    {
        public string Id { get; set; }
        public string OptionText { get; set; }
        public string NextQuestionId { get; set; }
    }

    public class SurveyResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
} 