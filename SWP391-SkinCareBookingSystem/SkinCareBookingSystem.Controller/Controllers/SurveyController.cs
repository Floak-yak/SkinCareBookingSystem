using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Repositories;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Service;
using SkinCareBookingSystem.Service.Interfaces;
using System.Collections.Generic;

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

        /// <summary>
        /// Get a specific question by ID
        /// </summary>
        /// <param name="questionId">ID of the question to retrieve</param>
        /// <returns>Question and its options</returns>
        [HttpGet("question/{questionId}")]
        public ActionResult<object> GetQuestion(string questionId)
        {
            var (question, options) = _surveyService.GetQuestion(questionId);
            if (question == null)
                return NotFound("Question not found");

            return Ok(new { question, options });
        }

        /// <summary>
        /// Get the next question ID based on the current question and chosen option
        /// </summary>
        /// <param name="currentQuestionId">ID of the current question</param>
        /// <param name="optionIndex">Index of the selected option (0-based)</param>
        /// <returns>Next question ID</returns>
        [HttpGet("next")]
        public ActionResult<string> GetNextQuestion([FromQuery] string currentQuestionId, [FromQuery] string optionIndex)
        {
            var nextQuestionId = _surveyService.GetNextQuestionId(currentQuestionId, optionIndex);
            if (nextQuestionId == null)
                return NotFound("Invalid question or option index");

            return Ok(new { nextQuestionId });
        }

        /// <summary>
        /// Check if a question is an end/result question
        /// </summary>
        /// <param name="questionId">ID of the question to check</param>
        /// <returns>True if it's an end question, false otherwise</returns>
        [HttpGet("isEndQuestion/{questionId}")]
        public ActionResult<bool> IsEndQuestion(string questionId)
        {
            return Ok(_surveyService.IsEndQuestion(questionId));
        }

        /// <summary>
        /// Get all questions in the survey
        /// </summary>
        /// <returns>Dictionary of all questions</returns>
        [HttpGet("all")]
        public ActionResult<Dictionary<string, SurveyQuestion>> GetAllQuestions()
        {
            return Ok(_surveyService.GetAllQuestions());
        }

        /// <summary>
        /// Update an existing question
        /// </summary>
        /// <param name="question">Updated question data</param>
        /// <returns>Success or failure response</returns>
        [HttpPut("update")]
        public IActionResult UpdateQuestion([FromBody] SurveyQuestion question)
        {
            if (question == null || string.IsNullOrEmpty(question.Id))
                return BadRequest("Invalid question data");

            var result = _surveyService.UpdateQuestion(question);
            if (result)
                return Ok(new { success = true, message = "Question updated successfully" });
            else
                return NotFound(new { success = false, message = "Question not found" });
        }

        /// <summary>
        /// Add a new question to the survey
        /// </summary>
        /// <param name="question">New question data</param>
        /// <returns>Success or failure response</returns>
        [HttpPost("add")]
        public IActionResult AddQuestion([FromBody] SurveyQuestion question)
        {
            if (question == null || string.IsNullOrEmpty(question.Id))
                return BadRequest("Invalid question data");

            var result = _surveyService.AddQuestion(question);
            if (result)
                return Ok(new { success = true, message = "Question added successfully" });
            else
                return BadRequest(new { success = false, message = "Question ID already exists" });
        }

        /// <summary>
        /// Delete a question from the survey
        /// </summary>
        /// <param name="questionId">ID of the question to delete</param>
        /// <returns>Success or failure response</returns>
        [HttpDelete("delete/{questionId}")]
        public IActionResult DeleteQuestion(string questionId)
        {
            var result = _surveyService.DeleteQuestion(questionId);
            if (result)
                return Ok(new { success = true, message = "Question deleted successfully" });
            else
                return NotFound(new { success = false, message = "Question not found" });
        }

        /// <summary>
        /// Get the first question to start the survey
        /// </summary>
        /// <returns>The first question</returns>
        [HttpGet("start")]
        public ActionResult<object> StartSurvey()
        {
            return GetQuestion("Q1");
        }
    }
}
