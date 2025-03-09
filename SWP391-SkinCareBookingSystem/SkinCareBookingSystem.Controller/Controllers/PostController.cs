using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.Service.Service;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("GetPosts")]
        public async Task<IActionResult> GetPosts() =>
            Ok(await _postService.GetAllPostsAsync());

        [HttpGet("GetPostById")]
        public async Task<IActionResult> GetPostById(int postId) =>
            Ok(await _postService.GetPostByIdAsync(postId));
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id) =>
            Ok(await _postService.DeletePost(id));
        [HttpPut("UpdatePost")]
        public async Task<IActionResult> UpdatePost(UpdatePostRequest request)
        {
            if (!await _postService.UpdatePost(request.Id, request.Title, request.CategoryId, request.imageLink))
                return BadRequest("Update fail");
            return Ok("Update Success");
        }

        [HttpPut("ApprovePost")]
        public async Task<IActionResult> ApprovePost(UpdatePostStatusRequest request)
        {
            if (!await _postService.ChangePostStatus(request.Id, request.status))
                return BadRequest("Update fail");
            return Ok("Update Success");
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreatePostWithContentsRequest request)
        {
            if (!await _postService.CreatePost(request.UserId, request.Title, request.contents, request.CategoryId, request.imageLink))
                return BadRequest("Create fail");
            return Ok("Create Success");
        }

        [HttpPost("CreateWithContent")]
        public async Task<IActionResult> CreateWithContent(CreatePostWithoutContentsRequest request)
        {
            if (!await _postService.CreatePostWithoutContent(request.UserId, request.Title, request.CategoryId, request.imageLink))
                return BadRequest("Create fail");
            return Ok("Create Success");
        }

        [HttpGet("CategoryId")]
        public async Task<IActionResult> SearchPostByCategoryId(int CategoryId)
        {
            List<Post> products = await _postService.Search(CategoryId);
            if (products is null)
                return NotFound();
            return Ok(products);
        }

        [HttpGet("IncludeName")]
        public async Task<IActionResult> SearchPostByName(string text)
        {
            List<Post> products = await _postService.Search(text);
            if (products is null)
                return NotFound();
            return Ok(products);
        }
    }
}
