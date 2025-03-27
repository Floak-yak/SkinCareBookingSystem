using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto.Survey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface ISurveyService
    {
        Task<Survey> GetQuestionByIdAsync(string questionId);
        Task<Survey> GetFirstQuestionAsync();
        Task<Survey> GetNextQuestionAsync(string currentQuestionId, string selectedOptionId);
        Task<Survey> GetResultByIdAsync(string resultId);
        Task<IEnumerable<Survey>> GetAllQuestionsAsync();
        Task<SurveyResponseDto> CreateSurveyAsync(CreateSurveyDto createSurveyDto);
        Task<SurveyResponseDto> UpdateSurveyAsync(string questionId, UpdateSurveyDto updateSurveyDto);
        Task<SurveyResponseDto> DeleteSurveyAsync(string questionId);
    }
}