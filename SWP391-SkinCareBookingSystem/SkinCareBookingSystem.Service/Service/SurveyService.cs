﻿using System;
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

        public async Task<List<SurveyOption>> GetOptionsForQuestionAsync(int questionId)
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
            return await _surveyRepository.GetSessionByIdAsync(id);
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

        public async Task<List<SurveyResponse>> GetResponsesBySessionIdAsync(int sessionId)
        {
            return await _surveyRepository.GetResponsesBySessionIdAsync(sessionId);
        }

        public async Task<List<RecommendedService>> GetRecommendedServicesByResultIdAsync(int resultId)
        {
            return await _surveyRepository.GetRecommendedServicesByResultIdAsync(resultId);
        }

        public async Task<List<SkincareService>> GetRecommendedServicesDetailsByResultIdAsync(int resultId)
        {
            var recommendedServices = await _surveyRepository.GetRecommendedServicesByResultIdAsync(resultId);
            if (recommendedServices == null || !recommendedServices.Any())
            {
                return new List<SkincareService>();
            }
            
            var serviceIds = recommendedServices.Select(rs => rs.ServiceId).ToList();
            
            var services = await _surveyRepository.GetServicesByIdsAsync(serviceIds);
            
            return services;
        }

        public async Task<SurveyQuestion> GetFirstQuestionAsync()
        {
            var allQuestions = await _surveyRepository.GetAllQuestionsAsync();
            return allQuestions.FirstOrDefault(q => q.QuestionId.StartsWith("Q1"));
        }

        public async Task<object> GetNextQuestionOrResultAsync(int sessionId, int currentQuestionId, int selectedOptionId)
        {
            // Record the response
            await RecordResponseAsync(sessionId, currentQuestionId, selectedOptionId);
            
            // Get the selected option
            var option = await _surveyRepository.GetOptionByIdAsync(selectedOptionId);
            if (option == null)
                return null;
            
            // If next question ID refers to a result, process completion
            var nextId = option.NextQuestionId;
            if (nextId.StartsWith("RESULT_"))
            {
                var result = await GetResultByResultIdAsync(nextId);
                if (result != null)
                {
                    await CompleteSessionAsync(sessionId, result.Id);
                    return new { 
                        isResult = true,
                        result = result,
                        recommendedServices = await GetRecommendedServicesDetailsByResultIdAsync(result.Id)
                    };
                }
            }
            else
            {
                // Get the next question
                var nextQuestion = await GetQuestionByQuestionIdAsync(nextId);
                if (nextQuestion != null)
                {
                    var options = await GetOptionsForQuestionAsync(nextQuestion.Id);
                    return new {
                        isResult = false,
                        questionId = nextQuestion.Id,
                        question = nextQuestion.QuestionText,
                        options = options
                    };
                }
            }
            
            return null;
        }

        public async Task<SurveyResult> ProcessSurveyCompletionAsync(int sessionId, string resultId)
        {
            var result = await GetResultByResultIdAsync(resultId);
            if (result == null)
                return null;
                
            await CompleteSessionAsync(sessionId, result.Id);
            return result;
        }

        public async Task<SkincareService> GetServiceByIdAsync(int serviceId)
        {
            return await _surveyRepository.GetServiceByIdAsync(serviceId);
        }

        public async Task AddRecommendedServiceAsync(int surveyResultId, int serviceId, int priority)
        {
            // Logic to add a recommended service
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
            var existingService = (await _surveyRepository.GetRecommendedServicesByResultIdAsync(id))
                .FirstOrDefault(s => s.Id == id);

            if (existingService == null)
            {
                throw new KeyNotFoundException($"Recommended service with ID {id} not found");
            }

            existingService.ServiceId = serviceId;
            existingService.Priority = priority;

            await _surveyRepository.UpdateRecommendedServiceAsync(existingService);
        }
    }
}
