using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class SurveyResultsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ISurveyService _surveyService;

        public SurveyResultsController(AppDbContext context, ISurveyService surveyService)
        {
            _context = context;
            _surveyService = surveyService;
        }

        // GET: api/SurveyResults/recommended-service
        [HttpGet("recommended-service")]
        public async Task<IActionResult> GetRecommendedServices()
        {
            try
            {
                var recommendedServices = await _context.RecommendedServices
                    .Include(rs => rs.Service)
                    .ToListAsync();

                var result = recommendedServices.Select(rs => new
                {
                    id = rs.Id,
                    recommendationId = rs.Id,
                    surveyResultId = rs.SurveyResultId,
                    serviceId = rs.ServiceId,
                    priority = rs.Priority,
                    service = rs.Service != null ? new
                    {
                        id = rs.Service.Id,
                        name = rs.Service.ServiceName,
                        description = rs.Service.ServiceDescription,
                        price = rs.Service.Price
                    } : null
                });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }

        // DELETE: api/SurveyResults/recommended-service/{id}
        [HttpDelete("recommended-service/{id}")]
        public async Task<IActionResult> DeleteRecommendedService(int id)
        {
            try
            {
                Console.WriteLine($"Attempting to delete recommended service with Service ID: {id}");
                
                // Find the recommended service by ServiceId instead of Id
                var recommendedService = await _context.RecommendedServices
                    .Include(rs => rs.Service)
                    .FirstOrDefaultAsync(rs => rs.ServiceId == id);
                
                if (recommendedService == null)
                {
                    Console.WriteLine($"Recommended service with Service ID {id} not found");
                    
                    // List all available service IDs for debugging
                    var availableServiceIds = await _context.RecommendedServices.Select(rs => rs.ServiceId).ToListAsync();
                    Console.WriteLine($"Available service IDs: {string.Join(", ", availableServiceIds)}");
                    
                    return NotFound(new { success = false, message = $"Recommended service with Service ID {id} not found" });
                }

                Console.WriteLine($"Found recommendation. Recommendation ID: {recommendedService.Id}, Service ID: {recommendedService.ServiceId}, Survey Result ID: {recommendedService.SurveyResultId}");
                
                // Remove the recommended service
                _context.RecommendedServices.Remove(recommendedService);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"Successfully deleted recommendation with Service ID: {id}");
                return Ok(new { success = true, message = "Recommended service removed successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting recommendation with Service ID {id}: {ex.Message}");
                return StatusCode(500, new { success = false, message = $"Internal server error: {ex.Message}" });
            }
        }
    }
} 