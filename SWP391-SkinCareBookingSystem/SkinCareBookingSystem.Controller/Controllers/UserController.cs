using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SkinCareBookingSystem.BusinessObject.Dto;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.Service.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers() =>
            Ok(await _userService.GetUsers());

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginReqeust request)
        {
            var user = await _userService.Login(request.Email, request.Password);

            if (user is null)
                return Unauthorized();

            var token = await _userService.GenerateToken(user);

            return Ok(token);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request is null)
                return BadRequest(new ArgumentNullException().Message) ;

            if (!await _userService.Register((Role)4, request.Email, request.Password, request.FullName, request.YearOfBirth, request.PhoneNumber))
                return BadRequest("Register fail");

            return Ok("Register success");
        }

        [HttpPut("Verify")]
        public async Task<IActionResult> VerifyAccount(string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Can't find token");
            if (!await _userService.VerifyAccount(token))
                return BadRequest("Verify fail");
            return Ok("Verify success");
        }
    }
}
