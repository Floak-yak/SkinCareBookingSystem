using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Repositories;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Service;
using SkinCareBookingSystem.Service.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class SurveyController : ControllerBase
{
    private readonly ISurveyService _surveyService;
    private readonly ISurveyRepository _surveyRepository;

    public SurveyController(ISurveyService surveyService, ISurveyRepository surveyRepository)
    {
        _surveyService = surveyService;
        _surveyRepository = surveyRepository;
    }

    [HttpGet("question/{questionId}")]
    public ActionResult<object> GetQuestion(string questionId)
    {
        var (question, choices) = _surveyService.GetQuestion(questionId);
        if (question == null)
            return NotFound("Question not found");

        return Ok(new { question, choices });
    }

    [HttpGet("next")]
    public ActionResult<string> GetNextQuestion([FromQuery] string currentQuestionId, [FromQuery] string choice)
    {
        var nextQuestionId = _surveyService.GetNextQuestionId(currentQuestionId, choice);
        if (nextQuestionId == null)
            return NotFound("Invalid question or choice");

        return Ok(nextQuestionId);
    }

    [HttpPost("update")]
    public IActionResult UpdateSurvey([FromBody] Node updatedNode)
    {
        var surveyTree = _surveyRepository.LoadSurvey();

        if (surveyTree.ContainsKey(updatedNode.Id))
        {
            surveyTree[updatedNode.Id] = updatedNode;
            _surveyRepository.SaveSurvey(surveyTree);
            return Ok("Cập nhật thành công!");
        }

        return NotFound("Không tìm thấy câu hỏi.");
    }
}
