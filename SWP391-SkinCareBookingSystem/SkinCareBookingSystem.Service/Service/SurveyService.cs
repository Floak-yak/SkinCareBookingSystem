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
            
            var allQuestions = await _surveyRepository.GetAllQuestionsAsync();
            var activeQuestions = allQuestions.Where(q => q.IsActive).ToList();
            
            var random = new Random();
            var selectedQuestions = activeQuestions
                .OrderBy(x => random.Next())
                .Take(10)
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
            var session = await _surveyRepository.GetSessionAsync(sessionId);
            if (session == null)
                return null;

            var responses = await _surveyRepository.GetResponsesAsync(sessionId);
            
            var answeredQuestionIds = responses.Select(r => r.QuestionId).ToList();
            
            var selectedQuestionId = session.SelectedQuestionId != null 
                ? session.SelectedQuestionId.Split(',').Select(int.Parse).ToList() 
                : new List<int>();
            
            var allQuestions = await _surveyRepository.GetAllQuestionsAsync();
            
            var nextQuestion = allQuestions
                .Where(q => selectedQuestionId.Contains(q.Id) && !answeredQuestionIds.Contains(q.Id) && q.IsActive)
                .OrderBy(_ => Guid.NewGuid())
                .FirstOrDefault();
                
            if (nextQuestion == null)
            {
                var skinTypeScores = await GetSkinTypeScoresAsync(sessionId);
                var maxScore = skinTypeScores.Max(s => s.Score);
                var topSkinTypes = skinTypeScores.Where(s => s.Score == maxScore).ToList();
                
                if (topSkinTypes.Count > 1 && answeredQuestionIds.Count >= 10)
                {
                    nextQuestion = allQuestions
                        .Where(q => !selectedQuestionId.Contains(q.Id) && !answeredQuestionIds.Contains(q.Id) && q.IsActive)
                        .OrderBy(_ => Guid.NewGuid())
                        .FirstOrDefault();
                        
                    if (nextQuestion != null)
                    {
                        selectedQuestionId.Add(nextQuestion.Id);
                        session.SelectedQuestionId = string.Join(",", selectedQuestionId);
                        await _surveyRepository.UpdateSessionAsync(session);
                    }
                }
                
                if (nextQuestion == null)
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
                    else
                    {
                        var finalSkinTypeScores = await GetSkinTypeScoresAsync(sessionId);
                        if (finalSkinTypeScores.Any())
                        {
                            var finalMaxScore = finalSkinTypeScores.Max(s => s.Score);
                            var topSkinTypeId = finalSkinTypeScores.OrderByDescending(s => s.Score).First().SkinTypeId;
                            
                            var allResults = await _surveyRepository.GetAllResultsAsync();
                            var matchingResult = allResults.FirstOrDefault(r => r.ResultId == topSkinTypeId);
                            
                            if (matchingResult != null)
                            {
                                await _surveyRepository.CompleteSessionAsync(sessionId, matchingResult.Id);
                                
                                return new
                                {
                                    success = true,
                                    isResult = true,
                                    isEnd = true,
                                    sessionId = session.Id,
                                    message = "Survey completed",
                                    skinType = matchingResult.SkinType,
                                    result = new
                                    {
                                        id = matchingResult.Id,
                                        resultId = matchingResult.ResultId,
                                        skinType = matchingResult.SkinType,
                                        resultText = matchingResult.ResultText,
                                        recommendationText = matchingResult.RecommendationText
                                    }
                                };
                            }
                        }
                        
                        return new
                        {
                            success = false,
                            isResult = true,
                            isEnd = true,
                            sessionId = session.Id,
                            message = "Survey completed no result."
                        };
                    }
                }
            }
            
            if (nextQuestion == null)
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
                        isEnd = true,
                        sessionId = session.Id,
                        message = "Survey completed.",
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
                else
                {
                    var finalSkinTypeScores = await GetSkinTypeScoresAsync(sessionId);
                    if (finalSkinTypeScores.Any())
                    {
                        var finalMaxScore = finalSkinTypeScores.Max(s => s.Score);
                        var topSkinTypeId = finalSkinTypeScores.OrderByDescending(s => s.Score).First().SkinTypeId;
                        
                        var allResults = await _surveyRepository.GetAllResultsAsync();
                        var matchingResult = allResults.FirstOrDefault(r => r.ResultId == topSkinTypeId);
                        
                        if (matchingResult != null)
                        {
                            await _surveyRepository.CompleteSessionAsync(sessionId, matchingResult.Id);
                            
                            return new
                            {
                                success = true,
                                isResult = true,
                                isEnd = true,
                                sessionId = session.Id,
                                message = "Survey completed",
                                skinType = matchingResult.SkinType,
                                result = new
                                {
                                    id = matchingResult.Id,
                                    resultId = matchingResult.ResultId,
                                    skinType = matchingResult.SkinType,
                                    resultText = matchingResult.ResultText,
                                    recommendationText = matchingResult.RecommendationText
                                }
                            };
                        }
                    }
                    
                    return new
                    {
                        success = false,
                        isResult = true,
                        isEnd = true,
                        sessionId = session.Id,
                        message = "Survey completed no result."
                    };
                }
            }
            else
            {
                var options = await _surveyRepository.GetOptionsForQuestionAsync(nextQuestion.Id);
                var formattedOptions = new List<object>();
                
                foreach (var option in options)
                {
                    // Get skin type points for this option
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
                    questionId = nextQuestion.Id,
                    questionText = nextQuestion.QuestionText,
                    options = formattedOptions
                };
            }
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
                            sessionId = session.Id,
                            skinTypeScores = skinTypeScores,
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
                        isEnd = true,
                        sessionId = sessionId,
                        message = "Survey completed",
                        skinType = matchingResult.SkinType
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
    }
}
