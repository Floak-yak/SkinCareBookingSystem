﻿using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Interfaces;
using System.Security.Claims;
using SkinCareBookingSystem.Service.Dto.Survey;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly ISurveyService _surveyService;

        public SurveyController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        [HttpGet("start")]
        public async Task<ActionResult<object>> StartSurvey()
        {
            try
            {
                var userId = GetCurrentUserId();
                var session = await _surveyService.StartSessionAsync(userId);
                
                var nextQuestionData = await _surveyService.GetNextQuestionAsync(session.Id);
                if (nextQuestionData == null)
                {
                    return NotFound("No survey questions found");
                }
                
                return Ok(new 
                { 
                    sessionId = session.Id,
                    nextQuestionData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("answer")]
        public async Task<ActionResult<object>> AnswerQuestion([FromBody] AnswerRequestDto request)
        {
            try
            {
                if (request.SessionId <= 0 || request.QuestionId <= 0 || request.OptionId <= 0)
                {
                    return BadRequest("Invalid request parameters");
                }

                var nextStep = await _surveyService.ProcessSurveyAnswerAsync(
                    request.SessionId, 
                    request.QuestionId, 
                    request.OptionId);
                
                if (nextStep == null)
                {
                    return BadRequest("Failed to process answer");
                }
                
                return Ok(nextStep);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("results/{sessionId}")]
        public async Task<ActionResult<object>> GetSessionResults(int sessionId)
        {
            try
            {
                var session = await _surveyService.GetSessionByIdAsync(sessionId);
                if (session == null)
                {
                    return NotFound("Survey session not found");
                }

                if (!session.IsCompleted || !session.SurveyResultId.HasValue)
                {
                    return BadRequest("Survey session not completed");
                }

                var result = await _surveyService.GetResultByIdAsync(session.SurveyResultId.Value);
                if (result == null)
                {
                    return NotFound("Survey result not found");
                }

                var recommendedServices = await _surveyService.GetRecommendedServicesDetailsAsync(result.Id);
                
                return Ok(new
                {
                    sessionId = session.Id,
                    completedDate = session.CompletedDate,
                    result = new
                    {
                        id = result.Id,
                        resultId = result.ResultId,
                        resultText = result.ResultText,
                        skinType = result.SkinType,
                        recommendationText = result.RecommendationText
                    },
                    recommendedServices = recommendedServices.Select(s => new
                    {
                        id = s.Id,
                        name = s.ServiceName,
                        description = s.ServiceDescription,
                        price = s.Price
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user-history")]
        public async Task<ActionResult<object>> GetUserSurveyHistory()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized("User not authenticated");
                }

                var sessions = await _surveyService.GetSessionsByUserIdAsync(userId.Value);
                var history = new List<object>();

                foreach (var session in sessions)
                {
                    if (session.SurveyResultId.HasValue)
                    {
                        var result = await _surveyService.GetResultByIdAsync(session.SurveyResultId.Value);
                        if (result != null)
                        {
                            history.Add(new
                            {
                                sessionId = session.Id,
                                completedDate = session.CompletedDate,
                                result = new
                                {
                                    skinType = result.SkinType,
                                    resultText = result.ResultText
                                }
                            });
                        }
                    }
                }

                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("verify")]
        public async Task<ActionResult<object>> VerifySurvey()
        {
            try
            {
                var errors = new List<string>();
                var warnings = new List<string>();

                var allQuestions = await _surveyService.GetAllQuestionsAsync();
                if (allQuestions.Count == 0)
                {
                    errors.Add("No survey questions found in the database");
                    return Ok(new { isValid = false, errors, warnings });
                }

                if (!allQuestions.Any(q => q.IsActive))
                {
                    errors.Add("No active questions found");
                }

                var allResults = await _surveyService.GetAllResultsAsync();
                if (allResults.Count == 0)
                {
                    errors.Add("No survey results found in the database");
                }

                foreach (var question in allQuestions)
                {
                    var options = await _surveyService.GetOptionAsync(question.Id);
                    
                    if (options.Count == 0)
                    {
                        errors.Add($"Question '{question.QuestionId}' has no options");
                    }
                    
                    foreach (var option in options)
                    {
                        if (string.IsNullOrEmpty(option.SkinTypeId))
                        {
                            warnings.Add($"Option '{option.OptionText}' (ID: {option.Id}) has no skin type associated");
                        }
                        else
                        {
                            var matchingResult = allResults.FirstOrDefault(r => r.ResultId == option.SkinTypeId);
                            if (matchingResult == null)
                            {
                                errors.Add($"Option '{option.OptionText}' (ID: {option.Id}) references non-existent skin type '{option.SkinTypeId}'");
                            }
                        }
                    }
                }

                foreach (var result in allResults)
                {
                    var services = await _surveyService.GetRecommendedServicesByResultIdAsync(result.Id);
                    if (services.Count == 0)
                    {
                        warnings.Add($"Result '{result.ResultId}' has no recommended services");
                    }
                }

                var isValid = errors.Count == 0;

                return Ok(new
                {
                    isValid,
                    errors,
                    warnings,
                    statistics = new
                    {
                        totalQuestions = allQuestions.Count,
                        totalResults = allResults.Count,
                        activeQuestions = allQuestions.Count(q => q.IsActive)
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error verifying survey: {ex.Message}");
            }
        }

        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<object>> GetSessionResponses(int sessionId)
        {
            try
            {
                var session = await _surveyService.GetSessionByIdAsync(sessionId);
                if (session == null)
                {
                    return NotFound("Session not found");
                }
                
                var responses = await _surveyService.GetSessionResponsesWithScoresAsync(sessionId);
                
                var skinTypeScores = await _surveyService.GetSkinTypeScoresAsync(sessionId);
                
                return Ok(new 
                {
                    sessionId = session.Id,
                    isCompleted = session.IsCompleted,
                    completedDate = session.CompletedDate,
                    responses = responses,
                    skinTypeScores = skinTypeScores.Select(s => new
                    {
                        skinTypeId = s.SkinTypeId,
                        score = s.Score
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #region Admin API
        [HttpGet("admin/questions")]
        public async Task<ActionResult<List<SurveyQuestion>>> GetAllQuestions()
        {
            try
            {
                var questions = await _surveyService.GetAllQuestionsAsync();
                return Ok(questions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("admin/question/{id}")]
        public async Task<ActionResult<SurveyQuestion>> GetQuestionById(int id)
        {
            try
            {
                var question = await _surveyService.GetQuestionByIdAsync(id);
                if (question == null)
                {
                    return NotFound("Question not found");
                }
                return Ok(question);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("admin/question")]
        public async Task<ActionResult<object>> AddQuestion([FromBody] QuestionRequestDto request)
        {
            try
            {
                var question = new SurveyQuestion
                {
                    QuestionId = request.QuestionId,
                    QuestionText = request.QuestionText,
                    IsActive = request.IsActive,
                    CreatedDate = DateTime.Now
                };

                var addedQuestion = await _surveyService.AddQuestionAsync(question);

                if (request.Options != null && request.Options.Any())
                {
                    foreach (var optionDto in request.Options)
                    {
                        var option = new SurveyOption
                        {
                            QuestionId = addedQuestion.Id,
                            OptionText = optionDto.OptionText,
                            Points = optionDto.Points,
                            SkinTypeId = optionDto.SkinTypeId
                        };
                        await _surveyService.AddOptionAsync(option);
                    }
                }

                var questionWithOptions = await _surveyService.GetQuestionByIdAsync(addedQuestion.Id);
                var options = await _surveyService.GetOptionAsync(addedQuestion.Id);

                return Ok(new
                {
                    id = questionWithOptions.Id,
                    questionId = questionWithOptions.QuestionId,
                    questionText = questionWithOptions.QuestionText,
                    isActive = questionWithOptions.IsActive,
                    createdDate = questionWithOptions.CreatedDate,
                    options = options.Select(o => new
                    {
                        id = o.Id,
                        optionText = o.OptionText,
                        points = o.Points,
                        skinTypeId = o.SkinTypeId
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("admin/question/{id}")]
        public async Task<ActionResult<object>> UpdateQuestion(int id, [FromBody] QuestionUpdateDto request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest("ID mismatch between URL and body");
                }

                var existingQuestion = await _surveyService.GetQuestionByIdAsync(id);
                if (existingQuestion == null)
                {
                    return NotFound("Question not found");
                }

                existingQuestion.QuestionId = request.QuestionId;
                existingQuestion.QuestionText = request.QuestionText;
                existingQuestion.IsActive = request.IsActive;

                var updatedQuestion = await _surveyService.UpdateQuestionAsync(existingQuestion);

                if (request.Options != null)
                {
                    var existingOptions = await _surveyService.GetOptionAsync(id);
                    
                    foreach (var optionDto in request.Options.Where(o => o.IsDeleted))
                    {
                        if (optionDto.Id.HasValue)
                        {
                            await _surveyService.DeleteOptionAsync(optionDto.Id.Value);
                        }
                    }

                    foreach (var optionDto in request.Options.Where(o => !o.IsDeleted))
                    {
                        if (optionDto.Id.HasValue)
                        {
                            var existingOption = existingOptions.FirstOrDefault(o => o.Id == optionDto.Id);
                            if (existingOption != null)
                            {
                                existingOption.OptionText = optionDto.OptionText;
                                existingOption.Points = optionDto.Points;
                                existingOption.SkinTypeId = optionDto.SkinTypeId;
                                await _surveyService.UpdateOptionAsync(existingOption);
                            }
                        }
                        else
                        {
                            var newOption = new SurveyOption
                            {
                                QuestionId = id,
                                OptionText = optionDto.OptionText,
                                Points = optionDto.Points,
                                SkinTypeId = optionDto.SkinTypeId
                            };
                            await _surveyService.AddOptionAsync(newOption);
                        }
                    }
                }

                var questionWithOptions = await _surveyService.GetQuestionByIdAsync(id);
                var options = await _surveyService.GetOptionAsync(id);

                return Ok(new
                {
                    id = questionWithOptions.Id,
                    questionId = questionWithOptions.QuestionId,
                    questionText = questionWithOptions.QuestionText,
                    isActive = questionWithOptions.IsActive,
                    createdDate = questionWithOptions.CreatedDate,
                    options = options.Select(o => new
                    {
                        id = o.Id,
                        optionText = o.OptionText,
                        points = o.Points,
                        skinTypeId = o.SkinTypeId
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("admin/question/{id}")]
        public async Task<ActionResult> DeleteQuestion(int id)
        {
            try
            {
                var result = await _surveyService.DeleteQuestionAsync(id);
                if (!result)
                {
                    return NotFound("Question not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("admin/results")]
        public async Task<IActionResult> GetSurveyResultsWithRecommendedServices()
        {
            try
            {
                var results = await _surveyService.GetAllResultsAsync();

                var enrichedResults = new List<object>();

                foreach (var result in results)
                {
                    var recommendedServices = await _surveyService.GetRecommendedServicesDetailsAsync(result.Id);

                    enrichedResults.Add(new
                    {
                        result.Id,
                        result.ResultId,
                        result.SkinType,
                        result.ResultText,
                        result.RecommendationText,
                        RecommendedServices = recommendedServices.Select(service => new
                        {
                            service.Id,
                            service.ServiceName,
                            service.ServiceDescription,
                            service.Price
                        })
                    });
                }

                return Ok(enrichedResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching survey results.", error = ex.Message });
            }
        }

        [HttpPost("admin/recommended-service")]
        public async Task<IActionResult> AddRecommendedService([FromBody] RecommendedServiceDto dto)
        {
            try
            {
                await _surveyService.AddRecommendedServiceAsync(dto.SurveyResultId, dto.ServiceId, dto.Priority);
                return Ok("Recommended service added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("admin/results/{id}")]
        public async Task<ActionResult<object>> UpdateSurveyResult(int id, [FromBody] SurveyResultUpdateDto request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest("ID mismatch between URL and body.");
                }

                var existingResult = await _surveyService.GetResultByIdAsync(id);
                if (existingResult == null)
                {
                    return NotFound("Survey result not found.");
                }

                existingResult.ResultId = request.ResultId;
                existingResult.ResultText = request.ResultText;
                existingResult.SkinType = request.SkinType;
                existingResult.RecommendationText = request.RecommendationText;

                var updatedResult = await _surveyService.UpdateResultAsync(existingResult);

                if (request.RecommendedServices != null)
                {
                    var existingServices = await _surveyService.GetRecommendedServicesByResultIdAsync(id);
                    
                    foreach (var serviceDto in request.RecommendedServices.Where(s => s.IsDeleted))
                    {
                        if (serviceDto.Id.HasValue)
                        {
                            await _surveyService.DeleteRecommendedServiceAsync(serviceDto.Id.Value);
                        }
                    }

                    foreach (var serviceDto in request.RecommendedServices.Where(s => !s.IsDeleted))
                    {
                        if (serviceDto.Id.HasValue)
                        {
                            var existingService = existingServices.FirstOrDefault(s => s.Id == serviceDto.Id);
                            if (existingService != null)
                            {
                                await _surveyService.UpdateRecommendedServiceAsync(
                                    serviceDto.Id.Value,
                                    serviceDto.ServiceId,
                                    serviceDto.Priority);
                            }
                        }
                        else
                        {
                            await _surveyService.AddRecommendedServiceAsync(
                                id,
                                serviceDto.ServiceId,
                                serviceDto.Priority);
                        }
                    }
                }

                var resultWithServices = await _surveyService.GetResultByIdAsync(id);
                var recommendedServices = await _surveyService.GetRecommendedServicesDetailsAsync(id);

                return Ok(new
                {
                    id = resultWithServices.Id,
                    resultId = resultWithServices.ResultId,
                    resultText = resultWithServices.ResultText,
                    skinType = resultWithServices.SkinType,
                    recommendationText = resultWithServices.RecommendationText,
                    recommendedServices = recommendedServices.Select(s => new
                    {
                        id = s.Id,
                        serviceName = s.ServiceName,
                        serviceDescription = s.ServiceDescription,
                        price = s.Price
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the survey result.", error = ex.Message });
            }
        }
        #endregion
    }
}