using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.Service.Dto;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using System.IO;
using SkinCareBookingSystem.Service.Dto.SkincareServiceDto;
using Microsoft.AspNetCore.Http;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class SkincareServicesController : ControllerBase
    {
        private readonly ISkincareServicesService _skincareServicesService;
        private readonly IImageService _imageService;

        public SkincareServicesController(ISkincareServicesService skincareServicesService, IImageService imageService)
        {
            _skincareServicesService = skincareServicesService;
            _imageService = imageService;
        }

        [HttpGet("GetServices")]
        public async Task<IActionResult> GetServices() =>
            Ok(await _skincareServicesService.GetServices());

        [HttpGet("GetServiceById")]
        public async Task<IActionResult> GetServiceById([FromQuery] int id)
        {
            var service = await _skincareServicesService.GetServiceByid(id);
            if (service == null)
                return NotFound("Service not found");
            return Ok(service);
        }

        [HttpGet("GetServiceByName")]
        public async Task<IActionResult> GetServiceByName([FromQuery] string name)
        {
            var service = await _skincareServicesService.GetServiceByname(name);
            if (service == null)
                return NotFound("Service not found");
            return Ok(service);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchServices([FromQuery] string search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrEmpty(search))
                return BadRequest("Search term cannot be empty");
            return Ok(await _skincareServicesService.Search(search, page, pageSize));
        }

        [HttpGet("GetRandomServicesByCategory")]
        public async Task<IActionResult> GetRandomServicesByCategory([FromQuery] int count = 3)
        {
            if (count <= 0)
                return BadRequest("Count must be greater than 0");
            
            var services = await _skincareServicesService.GetRandomServicesByCategory(count);
            if (services == null || !services.Any())
                return NotFound("No services found");
                
            return Ok(services);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateService([FromForm] SkincareServiceCreateDTO request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Request body cannot be null");
                }

                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest($"Invalid model state: {errors}");
                }

                var result = await _skincareServicesService.Create(
                    request.ServiceName,
                    request.ServiceDescription,
                    request.Price,
                    request.WorkTime,
                    request.CategoryId,
                    request.ImageId,
                    request.Benefits);

                if (!result)
                {
                    return BadRequest("Failed to create service.");
                }

                return Ok("Create service successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateService: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateService([FromQuery] int id, [FromForm] SkincareServiceUpdateDTO request)
        {
            try
            {
                if (request is null)
                    return BadRequest("Request body cannot be null");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _skincareServicesService.Update(
                    id,
                    request.ServiceName,
                    request.ServiceDescription,
                    request.Price,
                    request.WorkTime,
                    request.CategoryId,
                    request.ImageId,
                    request.Benefits);

                if (!result)
                    return BadRequest("Update service failed");

                return Ok("Service updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteService([FromQuery] int id)
        {
            try
            {
                if (!await _skincareServicesService.Delete(id))
                    return BadRequest("Delete service failed");

                return Ok("Delete service success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
} 