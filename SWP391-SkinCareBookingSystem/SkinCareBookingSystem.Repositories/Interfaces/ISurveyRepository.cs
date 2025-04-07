using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Interfaces
{
    public interface ISurveyRepository
    {
        Task<List<SurveyQuestion>> GetAllQuestionsAsync();
        Task<SurveyQuestion> GetQuestionByIdAsync(int id);
        Task<SurveyQuestion> GetQuestionByQuestionIdAsync(string questionId);
        Task<SurveyQuestion> AddQuestionAsync(SurveyQuestion question);
        Task<SurveyQuestion> UpdateQuestionAsync(SurveyQuestion question);
        Task<bool> DeleteQuestionAsync(int id);

        Task<List<SurveyOption>> GetOptionsForQuestionAsync(int questionId);
        Task<SurveyOption> GetOptionByIdAsync(int id);
        Task<SurveyOption> AddOptionAsync(SurveyOption option);
        Task<SurveyOption> UpdateOptionAsync(SurveyOption option);
        Task<bool> DeleteOptionAsync(int id);

        Task<List<SurveyResult>> GetAllResultsAsync();
        Task<SurveyResult> GetResultByIdAsync(int id);
        Task<SurveyResult> GetResultByResultIdAsync(string resultId);
        Task<SurveyResult> AddResultAsync(SurveyResult result);
        Task<SurveyResult> UpdateResultAsync(SurveyResult result);
        Task<bool> DeleteResultAsync(int id);

        Task<SurveySession> CreateSessionAsync(SurveySession session);
        Task<SurveySession> GetSessionAsync(int id);
        Task<List<SurveySession>> GetSessionsByUserIdAsync(int userId);
        Task<SurveySession> CompleteSessionAsync(int sessionId, int resultId);

        Task<SurveyResponse> AddResponseAsync(SurveyResponse response);
        Task<List<SurveyResponse>> GetResponsesAsync(int sessionId);

        Task<SkincareService> GetServiceByIdAsync(int serviceId);
        Task AddRecommendedServiceAsync(RecommendedService recommendedService);
        Task<List<RecommendedService>> GetRecommendedServicesAsync(int resultId);
        Task<List<SkincareService>> GetServicesByIdsAsync(List<int> serviceIds);
        Task<bool> DeleteRecommendedServiceAsync(int id);
        Task UpdateRecommendedServiceAsync(RecommendedService recommendedService);
        
        Task<UserSkinTypeScore> AddSkinTypeScoreAsync(UserSkinTypeScore score);
        Task<UserSkinTypeScore> UpdateSkinTypeScoreAsync(int sessionId, string skinTypeId, int pointsToAdd);
        Task<List<UserSkinTypeScore>> GetSkinTypeScoresAsync(int sessionId);
        Task<string> GetSkinTypeAsync(int sessionId);
    }
}
