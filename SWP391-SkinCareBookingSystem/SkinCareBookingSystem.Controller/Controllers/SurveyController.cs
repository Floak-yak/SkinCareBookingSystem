using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Interfaces;
using System.Security.Claims;
using SkinCareBookingSystem.Service.Dto.Survey;
using System.Linq;

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
                        var skinTypePoints = await _surveyService.GetOptionSkinTypePointsAsync(option.Id);
                        if (skinTypePoints == null || !skinTypePoints.Any())
                        {
                            warnings.Add($"Option '{option.OptionText}' (ID: {option.Id}) has no skin type points associated");
                        }
                        else
                        {
                            foreach (var skinTypePoint in skinTypePoints)
                            {
                                if (string.IsNullOrEmpty(skinTypePoint.SkinTypeId))
                                {
                                    errors.Add($"Option '{option.OptionText}' (ID: {option.Id}) has skin type point with empty skin type ID");
                                }
                                else
                                {
                                    var matchingResult = allResults.FirstOrDefault(r => r.ResultId == skinTypePoint.SkinTypeId);
                                    if (matchingResult == null)
                                    {
                                        errors.Add($"Option '{option.OptionText}' (ID: {option.Id}) has skin type point referencing non-existent skin type '{skinTypePoint.SkinTypeId}'");
                                    }
                                }
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
        public async Task<ActionResult<List<object>>> GetAllQuestions()
        {
            try
            {
                var questions = await _surveyService.GetAllQuestionsAsync();
                
                var result = questions.Select(q => new
                {
                    q.Id,
                    q.QuestionId,
                    q.QuestionText,
                    q.IsActive,
                    q.CreatedDate,
                    Options = q.Options.Select(o => new
                    {
                        o.Id,
                        o.OptionText,
                        o.QuestionId,
                        SkinTypePoints = o.SkinTypePoints.Select(sp => new
                        {
                            sp.Id,
                            sp.SkinTypeId,
                            sp.Points
                        }).ToList()
                    }).ToList()
                }).ToList();
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("admin/question/{id}")]
        public async Task<ActionResult<object>> GetQuestionById(int id)
        {
            try
            {
                var question = await _surveyService.GetQuestionByIdAsync(id);
                if (question == null)
                {
                    return NotFound("Question not found");
                }
                
                var result = new
                {
                    question.Id,
                    question.QuestionId,
                    question.QuestionText,
                    question.IsActive,
                    question.CreatedDate,
                    Options = question.Options.Select(o => new
                    {
                        o.Id,
                        o.OptionText,
                        o.QuestionId,
                        SkinTypePoints = o.SkinTypePoints.Select(sp => new
                        {
                            sp.Id,
                            sp.SkinTypeId,
                            sp.Points
                        }).ToList()
                    }).ToList()
                };
                
                return Ok(result);
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
                            OptionText = optionDto.OptionText
                        };
                        
                        var addedOption = await _surveyService.AddOptionAsync(option);
                        
                        if (optionDto.SkinTypePoints != null && optionDto.SkinTypePoints.Any())
                        {
                            foreach (var skinTypePoint in optionDto.SkinTypePoints)
                            {
                                var optionSkinTypePoints = new OptionSkinTypePoints
                                {
                                    OptionId = addedOption.Id,
                                    SkinTypeId = skinTypePoint.SkinTypeId,
                                    Points = skinTypePoint.Points
                                };
                                
                                await _surveyService.AddOptionSkinTypePointsAsync(optionSkinTypePoints);
                            }
                        }
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
                    options = await GetFormattedOptionsAsync(options)
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
                    
                    // Handle deleted options
                    foreach (var optionDto in request.Options.Where(o => o.IsDeleted))
                    {
                        if (optionDto.Id.HasValue)
                        {
                            // Delete all skin type points for this option first
                            var skinTypePoints = await _surveyService.GetOptionSkinTypePointsAsync(optionDto.Id.Value);
                            foreach (var point in skinTypePoints)
                            {
                                await _surveyService.DeleteOptionSkinTypePointsAsync(point.Id);
                            }
                            
                            await _surveyService.DeleteOptionAsync(optionDto.Id.Value);
                        }
                    }

                    // Handle updated or new options
                    foreach (var optionDto in request.Options.Where(o => !o.IsDeleted))
                    {
                        if (optionDto.Id.HasValue)
                        {
                            // Update existing option
                            var existingOption = existingOptions.FirstOrDefault(o => o.Id == optionDto.Id);
                            if (existingOption != null)
                            {
                                existingOption.OptionText = optionDto.OptionText;
                                await _surveyService.UpdateOptionAsync(existingOption);
                                
                                // Handle skin type points for existing option
                                var existingSkinTypePoints = await _surveyService.GetOptionSkinTypePointsAsync(existingOption.Id);
                                
                                // Delete removed skin type points
                                foreach (var point in existingSkinTypePoints)
                                {
                                    if (!optionDto.SkinTypePoints.Any(sp => sp.Id == point.Id && !sp.IsDeleted))
                                    {
                                        await _surveyService.DeleteOptionSkinTypePointsAsync(point.Id);
                                    }
                                }
                                
                                // Update or add skin type points
                                foreach (var skinTypePointDto in optionDto.SkinTypePoints.Where(sp => !sp.IsDeleted))
                                {
                                    if (skinTypePointDto.Id.HasValue)
                                    {
                                        // Update existing skin type point
                                        var existingPoint = existingSkinTypePoints.FirstOrDefault(p => p.Id == skinTypePointDto.Id);
                                        if (existingPoint != null)
                                        {
                                            existingPoint.SkinTypeId = skinTypePointDto.SkinTypeId;
                                            await _surveyService.UpdateOptionSkinTypePointsAsync(existingPoint);
                                        }
                                    }
                                    else
                                    {
                                        // Add new skin type point
                                        var newPoint = new OptionSkinTypePoints
                                        {
                                            OptionId = existingOption.Id,
                                            SkinTypeId = skinTypePointDto.SkinTypeId,
                                            Points = skinTypePointDto.Points
                                        };
                                        await _surveyService.AddOptionSkinTypePointsAsync(newPoint);
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Add new option
                            var newOption = new SurveyOption
                            {
                                QuestionId = id,
                                OptionText = optionDto.OptionText
                            };
                            var addedOption = await _surveyService.AddOptionAsync(newOption);
                            
                            // Add skin type points if provided
                            if (optionDto.SkinTypePoints != null && optionDto.SkinTypePoints.Any())
                            {
                                foreach (var skinTypePoint in optionDto.SkinTypePoints.Where(sp => !sp.IsDeleted))
                                {
                                    var optionSkinTypePoints = new OptionSkinTypePoints
                                    {
                                        OptionId = addedOption.Id,
                                        SkinTypeId = skinTypePoint.SkinTypeId,
                                        Points = skinTypePoint.Points
                                    };
                                    
                                    await _surveyService.AddOptionSkinTypePointsAsync(optionSkinTypePoints);
                                }
                            }
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
                    options = await GetFormattedOptionsAsync(options)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Helper method to format options with their skin type points
        private async Task<List<object>> GetFormattedOptionsAsync(List<SurveyOption> options)
        {
            var formattedOptions = new List<object>();
            
            foreach (var option in options)
            {
                var skinTypePoints = await _surveyService.GetOptionSkinTypePointsAsync(option.Id);
                
                formattedOptions.Add(new
                {
                    id = option.Id,
                    optionText = option.OptionText,
                    skinTypePoints = skinTypePoints.Select(sp => new
                    {
                        id = sp.Id,
                        skinTypeId = sp.SkinTypeId,
                        points = sp.Points
                    }).ToList()
                });
            }
            
            return formattedOptions;
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
        
        [HttpPost("admin/results")]
        public async Task<ActionResult<object>> CreateSurveyResult([FromBody] SurveyResultUpdateDto request)
        {
            try
            {
                var newResult = new SurveyResult
                {
                    ResultId = request.ResultId,
                    ResultText = request.ResultText,
                    SkinType = request.SkinType,
                    RecommendationText = request.RecommendationText,
                    CreatedDate = DateTime.Now
                };

                var createdResult = await _surveyService.AddResultAsync(newResult);

                if (request.RecommendedServices != null && request.RecommendedServices.Any())
                {
                    foreach (var serviceDto in request.RecommendedServices.Where(s => !s.IsDeleted))
                    {
                        await _surveyService.AddRecommendedServiceAsync(
                            createdResult.Id,
                            serviceDto.ServiceId,
                            serviceDto.Priority);
                    }
                }

                var resultWithServices = await _surveyService.GetResultByIdAsync(createdResult.Id);
                var recommendedServices = await _surveyService.GetRecommendedServicesDetailsAsync(createdResult.Id);

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
                return StatusCode(500, new { message = "An error creating result.", error = ex.Message });
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
        
        [HttpDelete("admin/results/{id}")]
        public async Task<ActionResult> DeleteSurveyResult(int id)
        {
            try
            {
                var existingResult = await _surveyService.GetResultByIdAsync(id);
                if (existingResult == null)
                {
                    return NotFound("Survey result not found.");
                }

                var recommendedServices = await _surveyService.GetRecommendedServicesByResultIdAsync(id);
                
                foreach (var service in recommendedServices)
                {
                    await _surveyService.DeleteRecommendedServiceAsync(service.Id);
                }
                
                await _surveyService.DeleteResultAsync(id);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error deleting result.", error = ex.Message });
            }
        }
        #endregion
    }
}