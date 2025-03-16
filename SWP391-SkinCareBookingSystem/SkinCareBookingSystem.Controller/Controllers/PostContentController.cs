using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.Service.Service;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostContentController : ControllerBase
    {
        private readonly IPostContentSerivce _postContentSerivce;

        public PostContentController(IPostContentSerivce postContentSerivce)
        {
            _postContentSerivce = postContentSerivce;
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetContents() =>
            Ok(await _postContentSerivce.GetContentsAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContentById(int id) =>
            Ok(await _postContentSerivce.GetContentByIdAsync(id));

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContent(int id) =>
            Ok(await _postContentSerivce.DeleteContent(id));

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreatePostContentRequest request)
        {
            if (!await _postContentSerivce.CreateContent(request))
                return BadRequest("Create fail");
            return Ok("Create Success");
        }
    }
}
