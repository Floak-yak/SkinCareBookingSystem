using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Dto.User;
using SkinCareBookingSystem.Service.Interfaces;
using SkinCareBookingSystem.Service.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkinCareBookingSystem.Controller.Controllers
{
    [Route("api/[controller]")]
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

        [HttpGet("GetStaffs")]
        public async Task<IActionResult> GetStaffs() =>
            Ok(await _userService.GetStaffs());

        [HttpGet("GetSkinTherapists")]
        public async Task<IActionResult> GetSkinTherapists() =>
            Ok(await _userService.GetSkinTherapists());

        [HttpGet("GetCustomers")]
        public async Task<IActionResult> GetCustomers() =>
            Ok(await _userService.GetCustomers());

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginReqeust request)
        {
            var user = await _userService.Login(request.Email, request.Password);

            if (user is null)
                return Unauthorized();

            var token = await _userService.GenerateToken(user);

            LoginResponse response = new() { Token = token, UserId = user.Id };

            return Ok(response);
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

        [HttpPost("CreateAccount")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (request is null)
                return BadRequest(new ArgumentNullException().Message);           

            return Ok(await _userService.CreateAccount((Role)request.Role, request.Email, request.FullName, request.YearOfBirth, request.PhoneNumber));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromQuery] int userId, int role)
        {
            if (role < 1 || role > 4)
                return BadRequest("Invalid role");
            if (!await _userService.UpdateRole(userId, (Role)role))
                return BadRequest("Update fail");
            return Ok("Update success");
        }

        [HttpPut("Verify")]
        public async Task<IActionResult> VerifyAccount([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Can't find token");
            if (!await _userService.VerifyAccount(token))
                return BadRequest("Verify fail");
            return Ok("Verify success");
        }

        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Can't find email");
            if (!await _userService.ResetPassword(email))
            {
                return BadRequest("Reset fail");
            }
            return Ok("Reset success");
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
                return BadRequest("OldPassword, newPassword must no be empty");
            if (!await _userService.ChangePassword(userId, oldPassword, newPassword))
            {
                return BadRequest("Change password fail");
            }
            return Ok("Change password success");
        }
    }
}
