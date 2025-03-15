using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Dto.BookingDto;
using SkinCareBookingSystem.Service.Dto.Image;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.Service.Service;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ImageService _imageService;

        public ImageController(ImageService imageService, IMapper mapper)
        {
            _mapper = mapper;
            _imageService = imageService;
        }
        [HttpGet("Gets")]
        public async Task<IActionResult> Gets() =>
            Ok(await _imageService.GetImages());
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile image, string? description)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest(new InvalidOperationException().Message);
            }
            StoreImageResponse result = await _imageService.StoreImage(image, description);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest("Upload Fail");
        }
    }
}
