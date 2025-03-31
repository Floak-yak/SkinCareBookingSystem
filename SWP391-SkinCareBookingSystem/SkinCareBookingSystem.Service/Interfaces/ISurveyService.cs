using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkinCareBookingSystem.BusinessObject.Entity;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface ISurveyService
    {
        (string question, Dictionary<string, string> choices) GetQuestion(string questionId);
        string GetNextQuestionId(string currentQuestionId, string choice);
        bool IsEndQuestion(string questionId);

        Task<List<SurveyQuestion>> GetAllQuestionsAsync();
        Task<SurveyQuestion> GetQuestionByIdAsync(int id);
        Task<SurveyQuestion> GetQuestionByQuestionIdAsync(string questionId);
        Task<SurveyQuestion> AddQuestionAsync(SurveyQuestion question);
        Task<SurveyQuestion> UpdateQuestionAsync(SurveyQuestion question);
        Task<bool> DeleteQuestionAsync(int id);

        Task<List<SurveyOption>> GetOptionsForQuestionAsync(int questionId);
        Task<SurveyOption> AddOptionAsync(SurveyOption option);
        Task<SurveyOption> UpdateOptionAsync(SurveyOption option);
        Task<bool> DeleteOptionAsync(int id);

        Task<List<SurveyResult>> GetAllResultsAsync();
        Task<SurveyResult> GetResultByIdAsync(int id);
        Task<SurveyResult> GetResultByResultIdAsync(string resultId);
        Task<SurveyResult> AddResultAsync(SurveyResult result);
        Task<SurveyResult> UpdateResultAsync(SurveyResult result);
        Task<bool> DeleteResultAsync(int id);

        Task<SurveySession> StartSessionAsync(int? userId);
        Task<SurveySession> GetSessionByIdAsync(int id);
        Task<List<SurveySession>> GetSessionsByUserIdAsync(int userId);
        Task<SurveySession> CompleteSessionAsync(int sessionId, int resultId);

        Task<SurveyResponse> RecordResponseAsync(int sessionId, int questionId, int optionId);
        Task<List<SurveyResponse>> GetResponsesBySessionIdAsync(int sessionId);

        Task<SurveyQuestion> GetFirstQuestionAsync();
        Task<object> GetNextQuestionOrResultAsync(int sessionId, int currentQuestionId, int selectedOptionId);
        Task<SurveyResult> ProcessSurveyCompletionAsync(int sessionId, string resultId);
        Task<SkincareService> GetServiceByIdAsync(int serviceId);
        Task AddRecommendedServiceAsync(int surveyResultId, int serviceId, int priority);
        Task<List<RecommendedService>> GetRecommendedServicesByResultIdAsync(int resultId);
        Task<List<SkincareService>> GetRecommendedServicesDetailsByResultIdAsync(int resultId);
    }
}
