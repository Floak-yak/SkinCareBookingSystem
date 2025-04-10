﻿using Azure.Core;
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
using System.Text.RegularExpressions;

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

        [HttpGet("WhoAmI")]
        public async Task<IActionResult> WhoAmI()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated");
            }

            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user identity");
            }

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers() =>
            Ok(await _userService.GetUsers());

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById([FromQuery] int userId) =>
            Ok(await _userService.GetUserById(userId));

        [HttpGet("GetSkinTherapistsByCategoryId")]
        public async Task<IActionResult> GetSkinTherapistsByCategoryId([FromQuery] int categoryId) =>
            Ok(await _userService.GetSkinTherapistsByCategoryId(categoryId));

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
            var result = await _userService.CreateAccount(request);
            if (result is null)
            {
                return BadRequest("Create fail");
            }
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateRole([FromQuery] int userId, int role, int categoryId)
        {
            if (role < 1 || role > 4)
                return BadRequest("Invalid role");
            if (!await _userService.UpdateRole(userId, (Role)role, categoryId))
                return BadRequest("Update fail");
            return Ok("Update success");
        }

        [HttpGet("Verify")]
        public async Task<IActionResult> VerifyAccount([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
                return Redirect("http://localhost:3000/verify-fail");

            if (!await _userService.VerifyAccount(token))
                return Redirect("http://localhost:3000/verify-fail");

            return Redirect("http://localhost:3000/verify-success");
        }

        [HttpPut("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromQuery] string email)
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
                return BadRequest("OldPassword, newPassword must not be empty");
            if (!await _userService.ChangePassword(userId, oldPassword, newPassword))
            {
                return BadRequest("Change password fail");
            }
            return Ok("Change password success");
        }

        [HttpPut("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromForm] UpdateUserProfileRequest request)
        {
            if (request is null)
                return BadRequest("Request must not be null");

            if (request.userId <= 0)
                return BadRequest("Invalid userId");

            if (string.IsNullOrEmpty(request.FullName))
                return BadRequest("FullName must not be empty");

            if (request.YearOfBirth == default)
                return BadRequest("YearOfBirth must be a valid date");

            int age = DateTime.Now.Year - request.YearOfBirth.Year;
            if (age < 6 || age > 100)
                return BadRequest("Age must be between 6 and 100 years");

            if (string.IsNullOrEmpty(request.Email))
                return BadRequest("Email must not be empty");

            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(request.Email, emailPattern))
                return BadRequest("Email format is invalid");

            if (string.IsNullOrEmpty(request.PhoneNumber))
                return BadRequest("PhoneNumber must not be empty");

            if (string.IsNullOrEmpty(request.PaymentMethod))
                return BadRequest("PaymentMethod must not be empty");

            if (string.IsNullOrEmpty(request.PaymentNumber))
                return BadRequest("PaymentNumber must not be empty");

            if (request.Image == null || request.Image.Length == 0)
                return BadRequest("Image must be uploaded");
            try
            {
                if (!await _userService.UpdateUserProfile(request))
                {
                    return BadRequest("Update fail");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Ok("Update success");
        }

        [HttpPut("UpdateAvatarForUser")]
        public async Task<IActionResult> UpdateAvatarForUser(UploadAvatarForUserRequest request)
        {
            if (!await _userService.UploadAvatarForUser(request))
            {
                return BadRequest("Update fail");
            }
            return Ok("Update success");
        }

        [HttpPut("UpdateDescription")]
        public async Task<IActionResult> UpdateDescription(UpdateUserDescriptionRequest request)
        {
            if (!await _userService.UpdateUserDescription(request))
            {
                return BadRequest("Update fail");
            }
            return Ok("Update success");
        }

        [HttpDelete("Remove")]
        public async Task<IActionResult> RemoveUser([FromQuery] RemoveUserRequest request)
        {
            if (!await _userService.RemoveUser(request))
            {
                return BadRequest("Remove fail");
            }
            return Ok("Remove success");
        }
    }
}
