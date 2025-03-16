using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Mapper;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class ServicesDetailController : ControllerBase
    {
        private readonly IServicesDetailService _servicesDetailService;

        public ServicesDetailController(IServicesDetailService servicesDetailService)
        {
            _servicesDetailService = servicesDetailService ?? throw new ArgumentNullException(nameof(servicesDetailService));
        }

        [HttpGet]
        [Route("GetAllDetails")]
        public async Task<IActionResult> GetAllServicesDetails()
        {
            try
            {
                var servicesDetails = await _servicesDetailService.GetAllServicesDetailsAsync();
                if (servicesDetails == null || !servicesDetails.Any())
                {
                    return NotFound(new { success = false, message = "No service details found" });
                }
                return Ok(new { success = true, data = servicesDetails.Select(sd => sd.ToDTO()) });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllServicesDetails: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("GetDetailById/{id}")]
        public async Task<IActionResult> GetServicesDetailById(int id)
        {
            try
            {
                var servicesDetail = await _servicesDetailService.GetServicesDetailByIdAsync(id);
                if (servicesDetail == null)
                {
                    return NotFound(new { success = false, message = $"Service detail with ID {id} not found" });
                }
                return Ok(new { success = true, data = servicesDetail.ToDTO() });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetServicesDetailById: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("GetByService/{serviceId}")]
        public async Task<IActionResult> GetServicesDetailsByServiceId(int serviceId)
        {
            try
            {
                var servicesDetails = await _servicesDetailService.GetServicesDetailsByServiceIdAsync(serviceId);
                if (servicesDetails == null || !servicesDetails.Any())
                {
                    return NotFound(new { success = false, message = $"No service details found for service ID {serviceId}" });
                }
                return Ok(new { success = true, data = servicesDetails.Select(sd => sd.ToDTO()) });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetServicesDetailsByServiceId: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateServicesDetail([FromBody] CreateServicesDetailDTO createDto)
        {
            try
            {
                if (createDto == null)
                {
                    return BadRequest(new { success = false, message = "Request body cannot be null" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(new { success = false, message = $"Invalid model state: {errors}" });
                }

                var entity = createDto.ToEntity();
                var createdService = await _servicesDetailService.CreateServicesDetailAsync(entity);
                return CreatedAtAction(
                    nameof(GetServicesDetailById), 
                    new { id = createdService.Id }, 
                    new { success = true, data = createdService.ToDTO(), message = "Service detail created successfully" }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateServicesDetail: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateServicesDetail(int id, [FromBody] UpdateServicesDetailDTO updateDto)
        {
            try
            {
                if (updateDto == null)
                {
                    return BadRequest(new { success = false, message = "Request body cannot be null" });
                }

                var existingService = await _servicesDetailService.GetServicesDetailByIdAsync(id);
                if (existingService == null)
                {
                    return NotFound(new { success = false, message = $"Service detail with ID {id} not found" });
                }

                updateDto.UpdateEntity(existingService);
                var updatedService = await _servicesDetailService.UpdateServicesDetailAsync(existingService);
                return Ok(new { success = true, data = updatedService.ToDTO(), message = "Service detail updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateServicesDetail: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteServicesDetail(int id)
        {
            try
            {
                await _servicesDetailService.DeleteServicesDetailAsync(id);
                return Ok(new { success = true, message = "Service detail deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteServicesDetail: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }
    }
}