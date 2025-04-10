﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.BusinessObject.Entity;
using Microsoft.EntityFrameworkCore.Storage;
using SkinCareBookingSystem.Service.Dto.Survey;
using AutoMapper;

namespace SkinCareBookingSystem.Service.Service
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IMapper _mapper;

        public SurveyService(ISurveyRepository surveyRepository, IMapper mapper)
        {
            _surveyRepository = surveyRepository;
            _mapper = mapper;
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
                .OrderBy(q => q.Id)
                .FirstOrDefault();

            if (nextQuestion == null)
            {
                var skinTypeScores = await GetSkinTypeScoresAsync(sessionId);
                if (!skinTypeScores.Any())
                    return CreateSurveyCompletedResponse(session.Id, false, true, "Survey completed no result.");

                var maxScore = skinTypeScores.Max(s => s.Score);
                var topSkinTypes = skinTypeScores.Where(s => s.Score == maxScore).ToList();

                if (topSkinTypes.Count > 1 && answeredQuestionIds.Count >= 5)
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
                    return await CompleteSurveyAsync(sessionId, session);
                }
            }

            if (nextQuestion != null)
            {
                return await FormatQuestionWithOptionsAsync(nextQuestion);
            }

            return await CompleteSurveyAsync(sessionId, session);
        }

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

        private async Task<object> CompleteSurveyAsync(int sessionId, SurveySession session)
        {
            var result = await GetSkinTypeByScoreAsync(sessionId);
            if (result != null)
            {
                await _surveyRepository.CompleteSessionAsync(sessionId, result.Id);
                return CreateSurveyCompletedResponse(session.Id, true, true, "Survey completed.");
            }

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

            return CreateSurveyCompletedResponse(session.Id, false, true, "Survey completed no result.");
        }

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

        public async Task<bool> UpdateQuestion(SurveyQuestion surveyQuestion, QuestionUpdateDto request)
        {
            if (request == null) throw new ArgumentNullException("Request is null");
            var options = _mapper.Map<List<SurveyOption>>(request.Options);
            if (surveyQuestion.Options.Count < options.Count)
            {
                for (int i = surveyQuestion.Options.Count; i < options.Count; i++)
                {
                    surveyQuestion.Options.Add(options[i]);
                }
            }
            else if (surveyQuestion.Options.Count > options.Count)
            {
                for (int i = options.Count; i < surveyQuestion.Options.Count; i++)
                {
                    surveyQuestion.Options.Remove(surveyQuestion.Options.ToList()[i]);
                    if (surveyQuestion.Options.ToList()[i - 1].Responses != null)
                        foreach (var it in surveyQuestion.Options.ToList()[i - 1].Responses)
                        {
                            _surveyRepository.DeleteResponseAsync(it.Id);
                        }
                }
            }
            for (int i = 0; i < surveyQuestion.Options.Count; i++)
            {
                surveyQuestion.Options.ToList()[i].OptionText = options[i].OptionText;
                surveyQuestion.Options.ToList()[i].SkinTypePoints = options[i].SkinTypePoints;
            }
            var result = await _surveyRepository.UpdateQuestionAsync(surveyQuestion);
            if (result != null)
                return true;
            return false;
        }
    }
}
