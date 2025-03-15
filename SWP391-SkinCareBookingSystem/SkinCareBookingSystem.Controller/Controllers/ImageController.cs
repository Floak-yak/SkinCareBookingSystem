using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
    }
}
