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

    [HttpGet("start")]
    public ActionResult<string> StartSurvey()
    {
        var result = _surveyService.StartSurvey();
        return Ok(result);
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
