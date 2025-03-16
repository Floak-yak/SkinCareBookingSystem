using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        [HttpGet]
        [Route("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetCategories();
                if (categories == null || !categories.Any())
                {
                    return NotFound(new { success = false, message = "No categories found" });
                }
                return Ok(new { success = true, data = categories });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCategories: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("GetCategoryById")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            try
            {
                var category = await _categoryService.GetCategoryById(categoryId);
                if (category == null)
                {
                    return NotFound(new { success = false, message = "Category not found" });
                }
                return Ok(new { success = true, data = category });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCategoryById: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("CreateUserId")]
        public async Task<IActionResult> CreateCategoryUserId(string categoryName, int userId)
        {
            try
            {
                if (string.IsNullOrEmpty(categoryName))
                {
                    return BadRequest(new { success = false, message = "Category name cannot be empty" });
                }

                if (!await _categoryService.CreateCategoryUserId(categoryName, userId))
                {
                    return BadRequest(new { success = false, message = "Failed to create category" });
                }

                return Ok(new { success = true, message = "Category created successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateCategory: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateCategory(string categoryName)
        {
            try
            {
                if (string.IsNullOrEmpty(categoryName))
                {
                    return BadRequest(new { success = false, message = "Category name cannot be empty" });
                }

                if (!await _categoryService.CreateCategory(categoryName))
                {
                    return BadRequest(new { success = false, message = "Failed to create category" });
                }

                return Ok(new { success = true, message = "Category created successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateSimpleCategory: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal server error" });
            }
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
