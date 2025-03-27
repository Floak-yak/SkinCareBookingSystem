using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Api.Controllers
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

        [HttpGet("start")]
        public async Task<ActionResult<Survey>> GetFirstQuestion()
        {
            var question = await _surveyService.GetFirstQuestionAsync();
            if (question == null)
            {
                return NotFound("Survey starting question not found.");
            }
            return Ok(question);
        }

        [HttpGet("question/{id}")]
        public async Task<ActionResult<Survey>> GetQuestion(string id)
        {
            var question = await _surveyService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound($"Question with ID {id} not found.");
            }
            return Ok(question);
        }

        [HttpPost("next")]
        public async Task<ActionResult<Survey>> GetNextQuestion([FromBody] NextQuestionRequest request)
        {
            if (string.IsNullOrEmpty(request.CurrentQuestionId) || string.IsNullOrEmpty(request.SelectedOptionId))
            {
                return BadRequest("Current question ID and option ID required.");
            }

            var nextQuestion = await _surveyService.GetNextQuestionAsync(
                request.CurrentQuestionId,
                request.SelectedOptionId);

            if (nextQuestion == null)
            {
                return NotFound("Next question not found.");
            }

            return Ok(nextQuestion);
        }

        [HttpGet("isResult/{id}")]
        public async Task<ActionResult<bool>> IsResultQuestion(string id)
        {
            var isResult = await _surveyService.IsResultQuestionAsync(id);
            return Ok(isResult);
        }

        [HttpGet("result/{id}")]
        public async Task<ActionResult<Survey>> GetResult(string id)
        {
            var result = await _surveyService.GetResultByIdAsync(id);
            if (result == null)
            {
                return NotFound($"Result with ID {id} not found.");
            }
            
            if (result.IsResult)
            {
                result.Options = new List<Option>();
            }
            
            return Ok(result);
        }
    }
}