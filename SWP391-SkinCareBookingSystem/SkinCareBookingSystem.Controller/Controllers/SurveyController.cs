using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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

        [HttpGet]
        public async Task<ActionResult<SurveyResponseDto>> GetAll()
        {
            var questions = await _surveyService.GetAllQuestionsAsync();
            
            return Ok(new SurveyResponseDto
            {
                Success = true,
                Message = "Survey questions.",
                Data = questions
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyResponseDto>> GetById(string id)
        {
            var question = await _surveyService.GetQuestionByIdAsync(id);
            
            if (question == null)
            {
                return NotFound(new SurveyResponseDto
                {
                    Success = false,
                    Message = $"Question with ID {id} not found."
                });
            }
            
            return Ok(new SurveyResponseDto
            {
                Success = true,
                Message = "Survey question.",
                Data = question
            });
        }

        [HttpGet("start")]
        public async Task<ActionResult<SurveyResponseDto>> GetFirst()
        {
            var question = await _surveyService.GetFirstQuestionAsync();
            
            if (question == null)
            {
                return NotFound(new SurveyResponseDto
                {
                    Success = false,
                    Message = "Survey starting question not found."
                });
            }
            
            return Ok(new SurveyResponseDto
            {
                Success = true,
                Message = "Starting question.",
                Data = question
            });
        }

        [HttpPost("next")]
        public async Task<ActionResult<SurveyResponseDto>> GetNext([FromBody] NextQuestionRequest request)
        {
            if (string.IsNullOrEmpty(request.CurrentQuestionId) || string.IsNullOrEmpty(request.SelectedOptionId))
            {
                return BadRequest(new SurveyResponseDto
                {
                    Success = false,
                    Message = "Current question ID and option ID required."
                });
            }

            var nextQuestion = await _surveyService.GetNextQuestionAsync(
                request.CurrentQuestionId,
                request.SelectedOptionId);

            if (nextQuestion == null)
            {
                return NotFound(new SurveyResponseDto
                {
                    Success = false,
                    Message = "Next question not found."
                });
            }

            return Ok(new SurveyResponseDto
            {
                Success = true,
                Message = "Next question.",
                Data = nextQuestion
            });
        }

        [HttpGet("results/{id}")]
        public async Task<ActionResult<SurveyResponseDto>> GetResult(string id)
        {
            var result = await _surveyService.GetResultByIdAsync(id);
            
            if (result == null)
            {
                return NotFound(new SurveyResponseDto
                {
                    Success = false,
                    Message = $"Result with ID {id} not found."
                });
            }
            
            if (result.IsResult)
            {
                result.Options = new List<Option>();
            }
            
            return Ok(new SurveyResponseDto
            {
                Success = true,
                Message = "Result.",
                Data = result
            });
        }

        [HttpPost]
        public async Task<ActionResult<SurveyResponseDto>> Create([FromBody] CreateSurveyDto createSurveyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new SurveyResponseDto
                {
                    Success = false,
                    Message = "Invalid model state",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var response = await _surveyService.CreateSurveyAsync(createSurveyDto);
            
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return StatusCode(201, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SurveyResponseDto>> Update(string id, [FromBody] UpdateSurveyDto updateSurveyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new SurveyResponseDto
                {
                    Success = false,
                    Message = "Invalid model state",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                });
            }

            var response = await _surveyService.UpdateSurveyAsync(id, updateSurveyDto);
            
            if (!response.Success)
            {
                if (response.Message.Contains("not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<SurveyResponseDto>> Delete(string id)
        {
            var response = await _surveyService.DeleteSurveyAsync(id);
            
            if (!response.Success)
            {
                if (response.Message.Contains("not found"))
                {
                    return NotFound(response);
                }
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}