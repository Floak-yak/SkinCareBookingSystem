using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    public class SkincareServicesController : ControllerBase
    {
        private readonly ISkincareServicesService _skincareServicesService;

        public SkincareServicesController(ISkincareServicesService skincareServicesService)
        {
            _skincareServicesService = skincareServicesService;
        }

        [HttpGet("GetServices")]
        public async Task<IActionResult> GetServices() =>
            Ok(await _skincareServicesService.GetServices());

        [HttpGet("GetServiceById")]
        public async Task<IActionResult> GetServiceById([FromQuery] int id)
        {
            var service = await _skincareServicesService.GetServiceByid(id);
            if (service == null)
                return BadRequest("Service not found");
            return Ok(service);
        }

        [HttpGet("GetServiceByName")]
        public async Task<IActionResult> GetServiceByName([FromQuery] string name)
        {
            var service = await _skincareServicesService.GetServiceByname(name);
            if (service == null)
                return BadRequest("Service not found");
            return Ok(service);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchServices([FromQuery] string search)
        {
            if (string.IsNullOrEmpty(search))
                return BadRequest("Search term cannot be empty");
            return Ok(await _skincareServicesService.Search(search));
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
        public async Task<IActionResult> CreateService([FromBody] SkincareServiceCreateDTO request)
        {
            if (request is null)
                return BadRequest("Request body cannot be null");

            if (string.IsNullOrEmpty(request.ServiceName))
                return BadRequest("Service name cannot be empty");
            if (request.ServiceName.Length > 100)
                return BadRequest("Service name cannot exceed 100 characters");

            if (request.Price <= 0)
                return BadRequest("Price must be greater than 0");

            if (request.WorkTime == DateTime.MinValue)
                return BadRequest("Work time cannot be empty");
            if (request.WorkTime.TimeOfDay.TotalHours > 3.5)
                return BadRequest("Work time cannot exceed 3.5 hours");

            if (!await _skincareServicesService.Create(
                request.ServiceName,
                request.Price,
                request.WorkTime))
                return BadRequest("Create service failed");

            return Ok("Service created successfully");
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateService([FromQuery] int id, [FromBody] SkincareServiceUpdateDTO request)
        {
            if (request is null)
                return BadRequest("Request body cannot be null");

            if (!string.IsNullOrEmpty(request.ServiceName))
            {
                if (request.ServiceName.Length > 100)
                    return BadRequest("Service name cannot exceed 100 characters");
            }

            if (request.Price <= 0)
                return BadRequest("Price must be greater than 0");

            if (request.WorkTime == DateTime.MinValue)
                return BadRequest("Work time cannot be empty");
            if (request.WorkTime.TimeOfDay.TotalHours > 3.5)
                return BadRequest("Work time cannot exceed 3.5 hours");

            var result = await _skincareServicesService.Update(
                id,
                request.ServiceName,
                request.Price,
                request.WorkTime);
                return BadRequest("Update service failed");

            return Ok("Service updated successfully");
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteService([FromQuery] int id)
        {
            if (!await _skincareServicesService.Delete(id))
                return BadRequest("Delete service failed");

            return Ok("Delete service success");
        }
    }

    public class SkincareServiceCreateDTO
    {
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public DateTime WorkTime { get; set; }
    }

    public class SkincareServiceUpdateDTO
    {
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public DateTime WorkTime { get; set; }
    }
} 