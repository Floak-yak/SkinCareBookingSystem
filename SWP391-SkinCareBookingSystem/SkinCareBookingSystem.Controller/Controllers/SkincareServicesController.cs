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
        private readonly IImageService _imageService;

        public SkincareServicesController(ISkincareServicesService skincareServicesService, IImageService imageService)
        {
            _skincareServicesService = skincareServicesService;
            _imageService = imageService;
        }

        [HttpGet("GetServices")]
        public async Task<IActionResult> GetServices([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
            Ok(await _skincareServicesService.GetServices(page, pageSize));

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
        public async Task<IActionResult> CreateService([FromBody] SkincareServiceCreateDTO request)
        {
            if (request is null)
                return BadRequest("Request body cannot be null");

            if (string.IsNullOrEmpty(request.ServiceName))
                return BadRequest("Service name cannot be empty");
            if (request.ServiceName.Length > 100)
                return BadRequest("Service name cannot exceed 100 characters");

            if (string.IsNullOrEmpty(request.ServiceDescription))
                return BadRequest("Service description cannot be empty");
            if (request.ServiceDescription.Length > 500)
                return BadRequest("Service description cannot exceed 500 characters");

            if (request.Price <= 0)
                return BadRequest("Price must be greater than 0");

            if (request.WorkTime == DateTime.MinValue)
                return BadRequest("Work time cannot be empty");
            if (request.WorkTime.TimeOfDay.TotalHours > 3.5)
                return BadRequest("Work time cannot exceed 3.5 hours");

            if (request.CategoryId <= 0)
                return BadRequest("Invalid category ID");

            // Handle image upload if provided
            Image image = null;
            if (!string.IsNullOrEmpty(request.ImageLink))
            {
                var success = await _imageService.StoreImage(request.ImageLink);
                if (!success)
                    return BadRequest("Failed to store image");
                image = await _imageService.GetImageByDescription(System.IO.Path.GetFileName(request.ImageLink));
                if (image == null)
                    return BadRequest("Failed to retrieve stored image");
            }

            if (!await _skincareServicesService.Create(
                request.ServiceName,
                request.ServiceDescription,
                request.Price,
                request.WorkTime,
                request.CategoryId,
                image?.Id))
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

            if (!string.IsNullOrEmpty(request.ServiceDescription))
            {
                if (request.ServiceDescription.Length > 500)
                    return BadRequest("Service description cannot exceed 500 characters");
            }

            if (request.Price.HasValue && request.Price.Value <= 0)
                return BadRequest("Price must be greater than 0");

            if (request.WorkTime.HasValue)
            {
                if (request.WorkTime.Value == DateTime.MinValue)
                    return BadRequest("Work time cannot be empty");
                if (request.WorkTime.Value.TimeOfDay.TotalHours > 3.5)
                    return BadRequest("Work time cannot exceed 3.5 hours");
            }

            if (request.CategoryId.HasValue && request.CategoryId.Value <= 0)
                return BadRequest("Invalid category ID");

            // Handle image update if provided
            int? imageId = null;
            if (!string.IsNullOrEmpty(request.ImageLink))
            {
                var success = await _imageService.StoreImage(request.ImageLink);
                if (!success)
                    return BadRequest("Failed to store image");
                var image = await _imageService.GetImageByDescription(System.IO.Path.GetFileName(request.ImageLink));
                if (image == null)
                    return BadRequest("Failed to retrieve stored image");
                imageId = image.Id;
            }

            var result = await _skincareServicesService.Update(
                id,
                request.ServiceName,
                request.ServiceDescription,
                request.Price,
                request.WorkTime,
                request.CategoryId,
                imageId);

            if (!result)
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
        public string ServiceDescription { get; set; }
        public string ImageLink { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public DateTime WorkTime { get; set; }
    }

    public class SkincareServiceUpdateDTO
    {
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string ImageLink { get; set; }
        public int? CategoryId { get; set; }
        public decimal? Price { get; set; }
        public DateTime? WorkTime { get; set; }
    }
} 