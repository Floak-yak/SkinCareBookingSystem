using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.Service.Dto;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using System.IO;

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
        [Consumes("application/json")]
        public async Task<IActionResult> CreateService([FromBody] SkincareServiceCreateDTO request)
        {
            try
            {
                // Log the raw request body
                var rawRequest = await new StreamReader(Request.Body).ReadToEndAsync();
                Console.WriteLine($"Raw request body: {rawRequest}");

                if (request is null)
                    return BadRequest("Request body cannot be null");

                // Log the deserialized request
                Console.WriteLine($"Deserialized request: ServiceName={request.ServiceName}, Description={request.ServiceDescription}, CategoryId={request.CategoryId}, Price={request.Price}, WorkTime={request.WorkTime}");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("Update")]
        [Consumes("application/json")]
        public async Task<IActionResult> UpdateService([FromQuery] int id, [FromBody] SkincareServiceUpdateDTO request)
        {
            try
            {
                if (request is null)
                    return BadRequest("Request body cannot be null");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

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

    public class SkincareServiceCreateDTO
    {
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string ImageLink { get; set; }
        public int CategoryId { get; set; }
        public decimal Price { get; set; }
        public int WorkTime { get; set; }
    }

    public class SkincareServiceUpdateDTO
    {
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string ImageLink { get; set; }
        public int? CategoryId { get; set; }
        public decimal? Price { get; set; }
        public int? WorkTime { get; set; }
    }
} 