using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Route("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetCategoryById")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            var category = await _categoryService.GetCategoryById(categoryId);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateCategory(string categoryName, int userId)
        {
            if (!await _categoryService.CreateCategory(categoryName, userId))
                return BadRequest("Create failed");
            return Ok("Create Success");
        }

        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(int categoryId, string newCategoryName)
        {
            if (!await _categoryService.UpdateCategory(categoryId, newCategoryName))
                return BadRequest("Update failed");
            return Ok("Update Success");
        }

        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            if (!await _categoryService.DeleteCategory(categoryId))
                return BadRequest("Delete failed");
            return Ok("Delete Success");
        }
    }
}
