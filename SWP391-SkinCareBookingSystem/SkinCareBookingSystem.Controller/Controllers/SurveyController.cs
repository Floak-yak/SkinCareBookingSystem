using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Repositories;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Service;
using SkinCareBookingSystem.Service.Interfaces;
using System.Security.Claims;
using SkinCareBookingSystem.Service.Dto.Survey;
using SkinCareBookingSystem.Repositories.Data;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly ISurveyService _surveyService;
        private readonly ISurveyRepository _surveyRepository;
        private readonly AppDbContext _context;

        public SurveyController(ISurveyService surveyService, ISurveyRepository surveyRepository, AppDbContext context)
        {
            _surveyService = surveyService;
            _surveyRepository = surveyRepository;
            _context = context;
        }

        #region Existing File-based Survey API
        [HttpGet("question/{questionId}")]
        public ActionResult<object> GetQuestion(string questionId)
        {
            var (question, choices) = _surveyService.GetQuestion(questionId);
            if (question == null)
                return NotFound("Question not found");

            // Convert choices dictionary to array of objects
            var options = choices.Select((kvp, index) => new
            {
                label = kvp.Key,
                nextId = kvp.Value
            }).ToArray();

            return Ok(new { question, options });
        }

        [HttpGet("next")]
        public ActionResult<object> GetNextQuestion([FromQuery] string currentQuestionId, [FromQuery] int optionIndex)
        {
            var choices = _surveyService.GetQuestion(currentQuestionId).Item2;
            if (choices == null || optionIndex < 0 || optionIndex >= choices.Count)
                return BadRequest("Invalid question or option index");

            var choiceKey = choices.Keys.ElementAt(optionIndex);
            var nextQuestionId = choices[choiceKey];

            return Ok(new { nextQuestionId });
        }

        [HttpGet("isEndQuestion/{questionId}")]
        public ActionResult<bool> IsEndQuestion(string questionId)
        {
            return Ok(_surveyService.IsEndQuestion(questionId));
        }

        [HttpGet("start")]
        public ActionResult<object> StartSurvey()
        {
            return GetQuestion("Q1");
        }

        [HttpGet("all")]
        public ActionResult<Dictionary<string, object>> GetAllQuestions()
        {
            var surveyTree = _surveyRepository.LoadSurvey();
            var result = new Dictionary<string, object>();

            foreach (var node in surveyTree)
            {
                var options = node.Value.Choices.Select((kvp, index) => new
                {
                    label = kvp.Key,
                    nextId = kvp.Value
                }).ToArray();

                result[node.Key] = new { question = node.Value.Content, options };
            }

            return Ok(result);
        }

        [HttpPut("update")]
        public IActionResult UpdateQuestion([FromBody] Node updatedNode)
        {
            try
            {
                var surveyTree = _surveyRepository.LoadSurvey();

                if (surveyTree.ContainsKey(updatedNode.Id))
                {
                    surveyTree[updatedNode.Id] = updatedNode;
                    _surveyRepository.SaveSurvey(surveyTree);
                    return Ok("Updated successfully");
                }

                return NotFound("Question not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("add")]
        public IActionResult AddQuestion([FromBody] Node newNode)
        {
            try
            {
                var surveyTree = _surveyRepository.LoadSurvey();

                if (surveyTree.ContainsKey(newNode.Id))
                {
                    return BadRequest("Question ID already exists");
                }

                surveyTree[newNode.Id] = newNode;
                _surveyRepository.SaveSurvey(surveyTree);
                return Ok("Added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete/{questionId}")]
        public IActionResult DeleteQuestion(string questionId)
        {
            try
            {
                var surveyTree = _surveyRepository.LoadSurvey();

                if (!surveyTree.ContainsKey(questionId))
                {
                    return NotFound("Question not found");
                }

                surveyTree.Remove(questionId);
                _surveyRepository.SaveSurvey(surveyTree);
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("verify")]
        public ActionResult<object> VerifyFileSurvey()
        {
            try
            {
                var surveyTree = _surveyRepository.LoadSurvey();
                var errors = new List<string>();
                var warnings = new List<string>();

                // Check if the survey has a starting question (Q1)
                if (!surveyTree.ContainsKey("Q1"))
                {
                    errors.Add("Survey is missing starting question (Q1)");
                }

                // Get all questionIDs and result IDs
                var allIds = surveyTree.Keys.ToList();
                var questionIds = allIds.Where(id => !id.StartsWith("RESULT_")).ToList();
                var resultIds = allIds.Where(id => id.StartsWith("RESULT_")).ToList();

                // Check for referenced questions or results that don't exist
                foreach (var node in surveyTree.Values)
                {
                    foreach (var nextId in node.Choices.Values)
                    {
                        if (!allIds.Contains(nextId))
                        {
                            errors.Add($"Question '{node.Id}' references non-existent ID '{nextId}'");
                        }
                    }

                    // Check if node has no choices
                    if (node.Choices.Count == 0 && !node.Id.StartsWith("RESULT_"))
                    {
                        errors.Add($"Question '{node.Id}' has no choices");
                    }
                }

                // Check for questions that are not reachable from Q1
                HashSet<string> reachableIds = new HashSet<string>();
                Queue<string> toVisit = new Queue<string>();
                toVisit.Enqueue("Q1");

                while (toVisit.Count > 0)
                {
                    var currentId = toVisit.Dequeue();
                    if (reachableIds.Contains(currentId)) continue;
                    reachableIds.Add(currentId);

                    if (surveyTree.TryGetValue(currentId, out Node node))
                    {
                        foreach (var nextId in node.Choices.Values)
                        {
                            toVisit.Enqueue(nextId);
                        }
                    }
                }

                foreach (var id in allIds)
                {
                    if (!reachableIds.Contains(id) && !id.StartsWith("RESULT_"))
                    {
                        warnings.Add($"Question '{id}' is not reachable from Q1");
                    }
                }

                // Check for cycles in the survey
                foreach (var startId in questionIds)
                {
                    var visited = new HashSet<string>();
                    var path = new List<string>();
                    var cyclePath = DetectCycle(startId, surveyTree, visited, path);
                    if (cyclePath != null)
                    {
                        errors.Add($"Cycle detected: {string.Join(" → ", cyclePath)}");
                        break; // One cycle is enough to report
                    }
                }

                // Check for dead ends (questions that don't lead to a result)
                foreach (var id in questionIds)
                {
                    if (!LeadsToResult(id, surveyTree, new HashSet<string>()))
                    {
                        warnings.Add($"Question '{id}' does not lead to a result (possible dead end)");
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
                        totalQuestions = questionIds.Count,
                        totalResults = resultIds.Count,
                        unreachableQuestions = allIds.Count - reachableIds.Count
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error verifying survey: {ex.Message}");
            }
        }

        private List<string> DetectCycle(string currentId, Dictionary<string, Node> surveyTree, HashSet<string> visited, List<string> path)
        {
            if (currentId.StartsWith("RESULT_")) return null; // Results don't have next questions
            if (!surveyTree.ContainsKey(currentId)) return null; // Non-existent node
            
            if (visited.Contains(currentId))
            {
                // Find the start of the cycle
                int cycleStart = path.IndexOf(currentId);
                if (cycleStart >= 0)
                {
                    return path.GetRange(cycleStart, path.Count - cycleStart);
                }
                return null;
            }

            visited.Add(currentId);
            path.Add(currentId);

            var node = surveyTree[currentId];
            foreach (var nextId in node.Choices.Values)
            {
                var cyclePath = DetectCycle(nextId, surveyTree, visited, path);
                if (cyclePath != null)
                {
                    return cyclePath;
                }
            }

            // Backtrack
            path.RemoveAt(path.Count - 1);
            return null;
        }

        private bool LeadsToResult(string currentId, Dictionary<string, Node> surveyTree, HashSet<string> visited)
        {
            if (currentId.StartsWith("RESULT_")) return true;
            if (!surveyTree.ContainsKey(currentId) || visited.Contains(currentId)) return false;
            
            visited.Add(currentId);
            var node = surveyTree[currentId];
            
            foreach (var nextId in node.Choices.Values)
            {
                if (LeadsToResult(nextId, surveyTree, visited))
                {
                    return true;
                }
            }
            
            return false;
        }
        #endregion

        #region Database-backed Survey API
        // Get the currently logged in user ID (or null if anonymous)
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        [HttpGet("db/start")]
        public async Task<ActionResult<object>> StartDatabaseSurvey()
        {
            try
            {
                var userId = GetCurrentUserId();
                var session = await _surveyService.StartSessionAsync(userId);
                var firstQuestion = await _surveyService.GetFirstQuestionAsync();
                
                if (firstQuestion == null)
                {
                    return NotFound("No survey questions found");
                }

                var options = await _surveyService.GetOptionsForQuestionAsync(firstQuestion.Id);
                
                return Ok(new 
                { 
                    sessionId = session.Id,
                    questionId = firstQuestion.Id,
                    questionIdString = firstQuestion.QuestionId,
                    question = firstQuestion.QuestionText,
                    options = options.Select(o => new { id = o.Id, text = o.OptionText }).ToList()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("db/answer")]
        public async Task<ActionResult<object>> AnswerQuestion([FromBody] AnswerRequestDto request)
        {
            try
            {
                if (request.SessionId <= 0 || request.QuestionId <= 0 || request.OptionId <= 0)
                {
                    return BadRequest("Invalid request parameters");
                }

                // Record the response
                await _surveyService.RecordResponseAsync(request.SessionId, request.QuestionId, request.OptionId);
                
                // Get the next question or result
                var nextStep = await _surveyService.GetNextQuestionOrResultAsync(
                    request.SessionId, 
                    request.QuestionId, 
                    request.OptionId);
                
                return Ok(nextStep);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("db/results/{sessionId}")]
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

                var recommendedServices = await _surveyService.GetRecommendedServicesDetailsByResultIdAsync(result.Id);
                
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

        [HttpGet("db/user-history")]
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

        [HttpGet("db/verify")]
        public async Task<ActionResult<object>> VerifyDatabaseSurvey()
        {
            try
            {
                var errors = new List<string>();
                var warnings = new List<string>();

                // Get all questions and options
                var allQuestions = await _surveyService.GetAllQuestionsAsync();
                
                // Check if there are any questions
                if (allQuestions.Count == 0)
                {
                    errors.Add("No survey questions found in the database");
                    return Ok(new { isValid = false, errors, warnings });
                }

                // Check if there's a valid first question
                var firstQuestion = allQuestions.FirstOrDefault(q => q.QuestionId == "Q1" || q.QuestionId.StartsWith("Q1_"));
                if (firstQuestion == null)
                {
                    errors.Add("Missing first question (Q1) in the database");
                }

                // Get all results
                var allResults = await _surveyService.GetAllResultsAsync();
                if (allResults.Count == 0)
                {
                    errors.Add("No survey results found in the database");
                }

                // Check option references
                foreach (var question in allQuestions)
                {
                    var options = await _surveyService.GetOptionsForQuestionAsync(question.Id);
                    
                    if (options.Count == 0)
                    {
                        errors.Add($"Question '{question.QuestionId}' has no options");
                        continue;
                    }

                    foreach (var option in options)
                    {
                        var nextId = option.NextQuestionId;
                        
                        // Check if the next question/result exists
                        if (nextId.StartsWith("RESULT_"))
                        {
                            var resultExists = allResults.Any(r => r.ResultId == nextId);
                            if (!resultExists)
                            {
                                errors.Add($"Option '{option.OptionText}' in question '{question.QuestionId}' references non-existent result '{nextId}'");
                            }
                        }
                        else
                        {
                            var nextQuestionExists = allQuestions.Any(q => q.QuestionId == nextId);
                            if (!nextQuestionExists)
                            {
                                errors.Add($"Option '{option.OptionText}' in question '{question.QuestionId}' references non-existent question '{nextId}'");
                            }
                        }
                    }
                }

                // Check for unreachable questions
                var reachableQuestions = new HashSet<string>();
                if (firstQuestion != null)
                {
                    await FindReachableQuestions(firstQuestion.QuestionId, allQuestions, reachableQuestions);
                }

                foreach (var question in allQuestions)
                {
                    if (!reachableQuestions.Contains(question.QuestionId))
                    {
                        warnings.Add($"Question '{question.QuestionId}' is not reachable from the first question");
                    }
                }

                // Check for cycles (this is simplistic and might not detect all cycles)
                foreach (var question in allQuestions)
                {
                    var visited = new HashSet<string>();
                    var path = new List<string>();
                    var cyclePath = await DetectDatabaseCycle(question.QuestionId, allQuestions, visited, path);
                    if (cyclePath != null)
                    {
                        warnings.Add($"Potential cycle detected: {string.Join(" → ", cyclePath)}");
                        break; // One potential cycle is enough to report
                    }
                }

                // Check for missing recommended services for results
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
                        unreachableQuestions = allQuestions.Count - reachableQuestions.Count
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error verifying database survey: {ex.Message}");
            }
        }

        private async Task FindReachableQuestions(string questionId, List<SurveyQuestion> allQuestions, HashSet<string> reachable)
        {
            if (reachable.Contains(questionId) || questionId.StartsWith("RESULT_"))
                return;

            reachable.Add(questionId);
            
            var question = allQuestions.FirstOrDefault(q => q.QuestionId == questionId);
            if (question == null)
                return;

            var options = await _surveyService.GetOptionsForQuestionAsync(question.Id);
            foreach (var option in options)
            {
                await FindReachableQuestions(option.NextQuestionId, allQuestions, reachable);
            }
        }

        private async Task<List<string>> DetectDatabaseCycle(string currentId, List<SurveyQuestion> allQuestions, HashSet<string> visited, List<string> path)
        {
            if (currentId.StartsWith("RESULT_")) return null; // Results don't have next questions
            
            var question = allQuestions.FirstOrDefault(q => q.QuestionId == currentId);
            if (question == null) return null; // Non-existent question
            
            if (visited.Contains(currentId))
            {
                // Find the start of the cycle
                int cycleStart = path.IndexOf(currentId);
                if (cycleStart >= 0)
                {
                    return path.GetRange(cycleStart, path.Count - cycleStart);
                }
                return null;
            }

            visited.Add(currentId);
            path.Add(currentId);

            var options = await _surveyService.GetOptionsForQuestionAsync(question.Id);
            foreach (var option in options)
            {
                var cyclePath = await DetectDatabaseCycle(option.NextQuestionId, allQuestions, visited, path);
                if (cyclePath != null)
                {
                    return cyclePath;
                }
            }

            // Backtrack
            path.RemoveAt(path.Count - 1);
            return null;
        }

        #region Admin API
        [HttpGet("db/admin/questions")]
        public async Task<ActionResult<List<SurveyQuestion>>> GetAllDatabaseQuestions()
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

        [HttpGet("db/admin/question/{id}")]
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

        [HttpPost("db/admin/question")]
        public async Task<ActionResult<SurveyQuestion>> AddDatabaseQuestion([FromBody] SurveyQuestion question)
        {
            try
            {
                var result = await _surveyService.AddQuestionAsync(question);
                return CreatedAtAction(nameof(GetQuestionById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("db/admin/question/{id}")]
        public async Task<ActionResult<SurveyQuestion>> UpdateDatabaseQuestion(int id, [FromBody] SurveyQuestion question)
        {
            if (id != question.Id)
            {
                Console.WriteLine($"ID mismatch: URL ID = {id}, Body ID = {question.Id}");
                return BadRequest("ID mismatch");
            }

            Console.WriteLine($"Full Request Body: {System.Text.Json.JsonSerializer.Serialize(question)}");

            try
            {
                var result = await _surveyService.UpdateQuestionAsync(question);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("db/admin/question/{id}")]
        public async Task<ActionResult> DeleteDatabaseQuestion(int id)
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

        [HttpGet("db/admin/results")]
        public async Task<IActionResult> GetSurveyResultsWithRecommendedServices()
        {
            try
            {
                var results = await _surveyService.GetAllResultsAsync();

                var enrichedResults = new List<object>();

                foreach (var result in results)
                {
                    var recommendedServices = await _surveyService.GetRecommendedServicesDetailsByResultIdAsync(result.Id);

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

        [HttpPost("db/admin/recommended-service")]
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

        [HttpPut("db/admin/results/{id}")]
        public async Task<IActionResult> UpdateSurveyResult(int id, [FromBody] SurveyResult updatedResult)
        {
            try
            {
                if (id != updatedResult.Id)
                {
                    return BadRequest("ID mismatch between URL and body.");
                }

                var result = await _surveyService.UpdateResultAsync(updatedResult);
                if (result == null)
                {
                    return NotFound("Survey result not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the survey result.", error = ex.Message });
            }
        }
        #endregion
        #endregion
    }
}