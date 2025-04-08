﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.BusinessObject.Entity;
using Microsoft.EntityFrameworkCore.Storage;

namespace SkinCareBookingSystem.Service.Service
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;

        public SurveyService(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _surveyRepository.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _surveyRepository.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _surveyRepository.RollbackTransactionAsync();
        }

        public async Task<List<SurveyQuestion>> GetAllQuestionsAsync()
        {
            return await _surveyRepository.GetAllQuestionsAsync();
        }

        public async Task<SurveyQuestion> GetQuestionByIdAsync(int id)
        {
            return await _surveyRepository.GetQuestionByIdAsync(id);
        }

        public async Task<SurveyQuestion> AddQuestionAsync(SurveyQuestion question)
        {
            return await _surveyRepository.AddQuestionAsync(question);
        }

        public async Task<SurveyQuestion> UpdateQuestionAsync(SurveyQuestion question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));
            
            if (string.IsNullOrWhiteSpace(question.QuestionText))
                throw new ArgumentException("Question text cannot be empty");
            
            if (string.IsNullOrWhiteSpace(question.QuestionId))
                throw new ArgumentException("Question ID cannot be empty");
            
            var result = await _surveyRepository.UpdateQuestionAsync(question);
            if (result == null)
                throw new KeyNotFoundException($"Question with ID {question.Id} not found");
            
            return result;
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
            
            var allQuestions = await _surveyRepository.GetAllQuestionsAsync();
            var activeQuestions = allQuestions.Where(q => q.IsActive).ToList();
            
            var random = new Random();
            var selectedQuestions = activeQuestions
                .OrderBy(x => random.Next())
                .Take(5)
                .Select(q => q.Id)
                .ToList();
            
            session.SelectedQuestionId = string.Join(",", selectedQuestions);
            
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
            var option = await _surveyRepository.GetOptionByIdAsync(optionId);
            if (option == null)
                throw new ArgumentException($"Invalid option ID: {optionId}");
            
            var skinTypePoints = await _surveyRepository.GetOptionSkinTypePointsAsync(optionId);
            
            var primarySkinType = skinTypePoints.OrderByDescending(sp => sp.Points).FirstOrDefault();
            
            var response = new SurveyResponse
            {
                SessionId = sessionId,
                QuestionId = questionId,
                OptionId = optionId,
                SkinTypeId = primarySkinType?.SkinTypeId ?? string.Empty,
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
            // Get session information
            var session = await _surveyRepository.GetSessionAsync(sessionId);
            if (session == null)
                return null;

            // Get answered questions
            var responses = await _surveyRepository.GetResponsesAsync(sessionId);
            var answeredQuestionIds = responses.Select(r => r.QuestionId).ToList();
            
            // Get selected questions
            var selectedQuestionId = session.SelectedQuestionId != null 
                ? session.SelectedQuestionId.Split(',').Select(int.Parse).ToList() 
                : new List<int>();
            
            // Get all questions
            var allQuestions = await _surveyRepository.GetAllQuestionsAsync();
            
            // Try to find next question from pre-selected questions
            var nextQuestion = allQuestions
                .Where(q => selectedQuestionId.Contains(q.Id) && !answeredQuestionIds.Contains(q.Id) && q.IsActive)
                .OrderBy(q => q.Id)
                .FirstOrDefault();
            
            // If no pre-selected questions left, check if we need a tie-breaker question
            if (nextQuestion == null)
            {
                // Check if there's a tie in skin type scores
                var skinTypeScores = await GetSkinTypeScoresAsync(sessionId);
                if (!skinTypeScores.Any())
                    return CreateSurveyCompletedResponse(session.Id, false, true, "Survey completed no result.");
                
                var maxScore = skinTypeScores.Max(s => s.Score);
                var topSkinTypes = skinTypeScores.Where(s => s.Score == maxScore).ToList();
                
                // If there's a tie and we've answered at least 5 questions, find a tie-breaker
                if (topSkinTypes.Count > 1 && answeredQuestionIds.Count >= 5)
                {
                    nextQuestion = allQuestions
                        .Where(q => !selectedQuestionId.Contains(q.Id) && !answeredQuestionIds.Contains(q.Id) && q.IsActive)
                        .OrderBy(_ => Guid.NewGuid())
                        .FirstOrDefault();
                    
                    // Update session with the new tie-breaker question
                    if (nextQuestion != null)
                    {
                        selectedQuestionId.Add(nextQuestion.Id);
                        session.SelectedQuestionId = string.Join(",", selectedQuestionId);
                        await _surveyRepository.UpdateSessionAsync(session);
                    }
                }
                
                // If we still don't have a next question, complete the survey
                if (nextQuestion == null)
                {
                    return await CompleteSurveyAsync(sessionId, session);
                }
            }
            
            // Format and return the next question with its options
            if (nextQuestion != null)
            {
                return await FormatQuestionWithOptionsAsync(nextQuestion);
            }
            
            // If we get here with no question, complete the survey
            return await CompleteSurveyAsync(sessionId, session);
        }
        
        // Helper method to format a question with its options
        private async Task<object> FormatQuestionWithOptionsAsync(SurveyQuestion question)
        {
            var options = await _surveyRepository.GetOptionsForQuestionAsync(question.Id);
            var formattedOptions = new List<object>();
            
            foreach (var option in options)
            {
                var skinTypePoints = await _surveyRepository.GetOptionSkinTypePointsAsync(option.Id);
                
                formattedOptions.Add(new 
                {
                    id = option.Id,
                    text = option.OptionText,
                    skinTypePoints = skinTypePoints.Select(sp => new
                    {
                        id = sp.Id,
                        skinTypeId = sp.SkinTypeId,
                        points = sp.Points
                    }).ToList()
                });
            }
            
            return new
            {
                success = true,
                isResult = false,
                questionId = question.Id,
                questionText = question.QuestionText,
                options = formattedOptions
            };
        }
        
        // Helper method to complete the survey and return the result
        private async Task<object> CompleteSurveyAsync(int sessionId, SurveySession session)
        {
            // Try to determine skin type by score
            var result = await GetSkinTypeByScoreAsync(sessionId);
            if (result != null)
            {
                await _surveyRepository.CompleteSessionAsync(sessionId, result.Id);
                return CreateSurveyCompletedResponse(session.Id, true, true, "Survey completed.");
            }
            
            // If no clear skin type, find the top skin type by score
            var finalSkinTypeScores = await GetSkinTypeScoresAsync(sessionId);
            if (finalSkinTypeScores.Any())
            {
                var topSkinTypeId = finalSkinTypeScores.OrderByDescending(s => s.Score).First().SkinTypeId;
                var allResults = await _surveyRepository.GetAllResultsAsync();
                var matchingResult = allResults.FirstOrDefault(r => r.ResultId == topSkinTypeId);
                
                if (matchingResult != null)
                {
                    await _surveyRepository.CompleteSessionAsync(sessionId, matchingResult.Id);
                    return CreateSurveyCompletedResponse(session.Id, true, true, "Survey completed.");
                }
            }
            
            // If we get here, we couldn't determine a result
            return CreateSurveyCompletedResponse(session.Id, false, true, "Survey completed no result.");
        }
        
        // Helper method to create a survey completed response
        private object CreateSurveyCompletedResponse(int sessionId, bool success, bool isCompleted, string message)
        {
            return new
            {
                success,
                isResult = true,
                isCompleted,
                isEnd = true,
                sessionId,
                message
            };
        }

        public async Task<object> ProcessSurveyAnswerAsync(int sessionId, int questionId, int optionId)
        {
            var option = await _surveyRepository.GetOptionByIdAsync(optionId);
            if (option == null) 
                return null;
                
            var response = await RecordResponseAsync(sessionId, questionId, optionId);
            
            if (option.SkinTypePoints != null && option.SkinTypePoints.Any())
            {
                foreach (var skinTypePoint in option.SkinTypePoints)
                {
                    await UpdateSkinTypeScoreAsync(sessionId, skinTypePoint.SkinTypeId, skinTypePoint.Points);
                }
            }
            
            var session = await _surveyRepository.GetSessionAsync(sessionId);
            var responses = await _surveyRepository.GetResponsesAsync(sessionId);
            var answeredQuestionIds = responses.Select(r => r.QuestionId).ToList();
            
            var selectedQuestionId = session.SelectedQuestionId != null 
                ? session.SelectedQuestionId.Split(',').Select(int.Parse).ToList() 
                : new List<int>();
                
            bool allPreSelectedQuestionsAnswered = selectedQuestionId.All(id => answeredQuestionIds.Contains(id));
            
            if (allPreSelectedQuestionsAnswered)
            {
                var skinTypeScores = await GetSkinTypeScoresAsync(sessionId);
                var maxScore = skinTypeScores.Max(s => s.Score);
                var topSkinTypes = skinTypeScores.Where(s => s.Score == maxScore).ToList();
                
                if (topSkinTypes.Count == 1)
                {
                    var result = await GetSkinTypeByScoreAsync(sessionId);
                    if (result != null)
                    {
                        await _surveyRepository.CompleteSessionAsync(sessionId, result.Id);
                        var recommendedServices = await GetRecommendedServicesDetailsAsync(result.Id);
                        
                        return new
                        {
                            success = true,
                            isResult = true,
                            isCompleted = true,
                            sessionId = session.Id,
                            message = "Survey completed."
                        };
                    }
                }
            }
            
            var nextQuestion = await GetNextQuestionAsync(sessionId);
            
            if (nextQuestion == null)
            {
                var sessionSkinTypeScores = await GetSkinTypeScoresAsync(sessionId);
                var sessionMaxScore = sessionSkinTypeScores.Max(s => s.Score);
                var topSkinTypeId = sessionSkinTypeScores.OrderByDescending(s => s.Score).First().SkinTypeId;
                
                var allResults = await _surveyRepository.GetAllResultsAsync();
                var matchingResult = allResults.FirstOrDefault(r => r.ResultId == topSkinTypeId);
                
                if (matchingResult != null)
                {
                    await _surveyRepository.CompleteSessionAsync(sessionId, matchingResult.Id);
                    
                    return new
                    {
                        success = true,
                        isResult = true,
                        isCompleted = true,
                        isEnd = true,
                        sessionId = sessionId,
                        message = "Survey completed."
                    };
                }
                
                return new
                {
                    success = false,
                    isResult = true,
                    isEnd = true,
                    sessionId = sessionId,
                    message = "Survey completed failed."
                };
            }
            
            return nextQuestion;
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

        public async Task<List<object>> GetSessionResponsesWithScoresAsync(int sessionId)
        {
            var responses = await _surveyRepository.GetResponsesAsync(sessionId);
            if (responses == null || !responses.Any())
                return new List<object>();
            
            var result = new List<object>();
            
            foreach (var response in responses)
            {
                var question = await _surveyRepository.GetQuestionByIdAsync(response.QuestionId);
                var option = await _surveyRepository.GetOptionByIdAsync(response.OptionId);
                
                if (question != null && option != null)
                {
                    var skinTypePoints = await _surveyRepository.GetOptionSkinTypePointsAsync(option.Id);
                    result.Add(new
                    {
                        responseId = response.Id,
                        responseDate = response.ResponseDate,
                        question = new
                        {
                            id = question.Id,
                            questionId = question.QuestionId,
                            questionText = question.QuestionText
                        },
                        selectedOption = new
                        {
                            id = option.Id,
                            optionText = option.OptionText,
                            skinTypeId = response.SkinTypeId,
                            skinTypePoints = skinTypePoints.Select(sp => new
                            {
                                id = sp.Id,
                                skinTypeId = sp.SkinTypeId,
                                points = sp.Points
                            }).ToList()
                        }
                    });
                }
            }
            
            return result;
        }

        public async Task<List<OptionSkinTypePoints>> GetOptionSkinTypePointsAsync(int optionId)
        {
            return await _surveyRepository.GetOptionSkinTypePointsAsync(optionId);
        }

        public async Task<OptionSkinTypePoints> GetOptionSkinTypePointByIdAsync(int id)
        {
            return await _surveyRepository.GetOptionSkinTypePointByIdAsync(id);
        }

        public async Task<OptionSkinTypePoints> AddOptionSkinTypePointsAsync(OptionSkinTypePoints points)
        {
            return await _surveyRepository.AddOptionSkinTypePointsAsync(points);
        }

        public async Task<OptionSkinTypePoints> UpdateOptionSkinTypePointsAsync(OptionSkinTypePoints points)
        {
            return await _surveyRepository.UpdateOptionSkinTypePointsAsync(points);
        }

        public async Task<bool> DeleteOptionSkinTypePointsAsync(int id)
        {
            return await _surveyRepository.DeleteOptionSkinTypePointsAsync(id);
        }

        public async Task<List<SurveyResponse>> GetResponsesByOptionIdAsync(int optionId)
        {
            return await _surveyRepository.GetResponsesByOptionIdAsync(optionId);
        }

        public async Task<bool> DeleteResponseAsync(int responseId)
        {
            return await _surveyRepository.DeleteResponseAsync(responseId);
        }
    }
}
