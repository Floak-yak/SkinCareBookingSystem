using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.BusinessObject.Entity;

namespace SkinCareBookingSystem.Service.Service
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;

        public SurveyService(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }
        public async Task<List<SurveyQuestion>> GetAllQuestionsAsync()
        {
            return await _surveyRepository.GetAllQuestionsAsync();
        }

        public async Task<SurveyQuestion> GetQuestionByIdAsync(int id)
        {
            return await _surveyRepository.GetQuestionByIdAsync(id);
        }

        public async Task<SurveyQuestion> GetQuestionByQuestionIdAsync(string questionId)
        {
            return await _surveyRepository.GetQuestionByQuestionIdAsync(questionId);
        }

        public async Task<SurveyQuestion> AddQuestionAsync(SurveyQuestion question)
        {
            return await _surveyRepository.AddQuestionAsync(question);
        }

        public async Task<SurveyQuestion> UpdateQuestionAsync(SurveyQuestion question)
        {
            return await _surveyRepository.UpdateQuestionAsync(question);
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            return await _surveyRepository.DeleteQuestionAsync(id);
        }

        public async Task<List<SurveyOption>> GetOptionAsync(int questionId)
        {
            return await _surveyRepository.GetOptionsForQuestionAsync(questionId);
        }

        public async Task<SurveyOption> AddOptionAsync(SurveyOption option)
        {
            return await _surveyRepository.AddOptionAsync(option);
        }

        public async Task<SurveyOption> UpdateOptionAsync(SurveyOption option)
        {
            return await _surveyRepository.UpdateOptionAsync(option);
        }

        public async Task<bool> DeleteOptionAsync(int id)
        {
            return await _surveyRepository.DeleteOptionAsync(id);
        }

        public async Task<List<SurveyResult>> GetAllResultsAsync()
        {
            return await _surveyRepository.GetAllResultsAsync();
        }

        public async Task<SurveyResult> GetResultByIdAsync(int id)
        {
            return await _surveyRepository.GetResultByIdAsync(id);
        }

        public async Task<SurveyResult> GetResultByResultIdAsync(string resultId)
        {
            return await _surveyRepository.GetResultByResultIdAsync(resultId);
        }

        public async Task<SurveyResult> AddResultAsync(SurveyResult result)
        {
            return await _surveyRepository.AddResultAsync(result);
        }

        public async Task<SurveyResult> UpdateResultAsync(SurveyResult result)
        {
            return await _surveyRepository.UpdateResultAsync(result);
        }

        public async Task<bool> DeleteResultAsync(int id)
        {
            return await _surveyRepository.DeleteResultAsync(id);
        }

        public async Task<SurveySession> StartSessionAsync(int? userId)
        {
            var session = new SurveySession
            {
                UserId = userId,
                IsCompleted = false,
                CompletedDate = DateTime.Now
            };

            return await _surveyRepository.CreateSessionAsync(session);
        }

        public async Task<SurveySession> GetSessionByIdAsync(int id)
        {
            return await _surveyRepository.GetSessionAsync(id);
        }

        public async Task<List<SurveySession>> GetSessionsByUserIdAsync(int userId)
        {
            return await _surveyRepository.GetSessionsByUserIdAsync(userId);
        }

        public async Task<SurveySession> CompleteSessionAsync(int sessionId, int resultId)
        {
            return await _surveyRepository.CompleteSessionAsync(sessionId, resultId);
        }

        public async Task<SurveyResponse> RecordResponseAsync(int sessionId, int questionId, int optionId)
        {
            var response = new SurveyResponse
            {
                SessionId = sessionId,
                QuestionId = questionId,
                OptionId = optionId,
                ResponseDate = DateTime.Now
            };

            return await _surveyRepository.AddResponseAsync(response);
        }

        public async Task<List<RecommendedService>> GetRecommendedServicesByResultIdAsync(int resultId)
        {
            return await _surveyRepository.GetRecommendedServicesAsync(resultId);
        }

        public async Task<List<SkincareService>> GetRecommendedServicesDetailsAsync(int resultId)
        {
            var recommendedServices = await _surveyRepository.GetRecommendedServicesAsync(resultId);
            if (recommendedServices == null || !recommendedServices.Any())
            {
                return new List<SkincareService>();
            }
            
            var serviceIds = recommendedServices.Select(rs => rs.ServiceId).ToList();
            
            var services = await _surveyRepository.GetServicesByIdsAsync(serviceIds);
            
            return services;
        }

        public async Task<SkincareService> GetServiceByIdAsync(int serviceId)
        {
            return await _surveyRepository.GetServiceByIdAsync(serviceId);
        }

        public async Task AddRecommendedServiceAsync(int surveyResultId, int serviceId, int priority)
        {
            var recommendedService = new RecommendedService
            {
                SurveyResultId = surveyResultId,
                ServiceId = serviceId,
                Priority = priority
            };

            await _surveyRepository.AddRecommendedServiceAsync(recommendedService);
        }

        public async Task<bool> DeleteRecommendedServiceAsync(int id)
        {
            return await _surveyRepository.DeleteRecommendedServiceAsync(id);
        }

        public async Task UpdateRecommendedServiceAsync(int id, int serviceId, int priority)
        {
            var existingService = (await _surveyRepository.GetRecommendedServicesAsync(id))
                .FirstOrDefault(s => s.Id == id);

            if (existingService == null)
            {
                throw new KeyNotFoundException($"Recommended service with ID {id} not found");
            }

            existingService.ServiceId = serviceId;
            existingService.Priority = priority;

            await _surveyRepository.UpdateRecommendedServiceAsync(existingService);
        }

        public async Task<object> GetNextQuestionAsync(int sessionId)
        {
            var session = await _surveyRepository.GetSessionAsync(sessionId);
            if (session == null)
                return null;

            var responses = await _surveyRepository.GetResponsesAsync(sessionId);
            
            var answeredQuestionIds = responses.Select(r => r.QuestionId).ToList();
            
            var allQuestions = await _surveyRepository.GetAllQuestionsAsync();
            
            var nextQuestion = allQuestions
                .Where(q => !answeredQuestionIds.Contains(q.Id) && q.IsActive)
                .OrderBy(q => q.Id)
                .FirstOrDefault();

            if (nextQuestion == null)
            {
                var result = await GetSkinTypeByScoreAsync(sessionId);
                if (result != null)
                {
                    await _surveyRepository.CompleteSessionAsync(sessionId, result.Id);
                    var recommendedServices = await GetRecommendedServicesDetailsAsync(result.Id);
                    
                    return new
                    {
                        isResult = true,
                        sessionId = session.Id,
                        skinTypeScores = await GetSkinTypeScoresAsync(sessionId),
                        result = new
                        {
                            id = result.Id,
                            resultId = result.ResultId,
                            skinType = result.SkinType,
                            resultText = result.ResultText,
                            recommendationText = result.RecommendationText
                        },
                        recommendedServices = recommendedServices
                    };
                }
            }
            else
            {
                var options = await _surveyRepository.GetOptionsForQuestionAsync(nextQuestion.Id);
                return new
                {
                    isResult = false,
                    questionId = nextQuestion.Id,
                    questionText = nextQuestion.QuestionText,
                    options = options.Select(o => new 
                    {
                        id = o.Id,
                        text = o.OptionText,
                        skinTypeId = o.SkinTypeId,
                        points = o.Points
                    }).ToList()
                };
            }
            
            return null;
        }

        public async Task<object> ProcessSurveyAnswerAsync(int sessionId, int questionId, int optionId)
        {
            await RecordResponseAsync(sessionId, questionId, optionId);
            
            var option = await _surveyRepository.GetOptionByIdAsync(optionId);
            if (option == null)
                return null;
                
            if (!string.IsNullOrEmpty(option.SkinTypeId))
            {
                await UpdateSkinTypeScoreAsync(sessionId, option.SkinTypeId, option.Points);
            }
            
            return await GetNextQuestionAsync(sessionId);
        }

        public async Task<UserSkinTypeScore> UpdateSkinTypeScoreAsync(int sessionId, string skinTypeId, int pointsToAdd)
        {
            return await _surveyRepository.UpdateSkinTypeScoreAsync(sessionId, skinTypeId, pointsToAdd);
        }
        
        public async Task<List<UserSkinTypeScore>> GetSkinTypeScoresAsync(int sessionId)
        {
            return await _surveyRepository.GetSkinTypeScoresAsync(sessionId);
        }
        
        public async Task<SurveyResult> GetSkinTypeByScoreAsync(int sessionId)
        {
            var winningSkinTypeId = await _surveyRepository.GetSkinTypeAsync(sessionId);
            
            if (string.IsNullOrEmpty(winningSkinTypeId))
                return null;
                
            var allResults = await _surveyRepository.GetAllResultsAsync();
            var matchingResult = allResults.FirstOrDefault(r => r.ResultId == winningSkinTypeId);
            
            return matchingResult;
        }
    }
}
