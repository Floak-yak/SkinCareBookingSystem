using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Interfaces;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkinCareBookingSystem.Repositories.Repositories;
using AutoMapper;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Dto.User;
using Azure.Core;

namespace SkinCareBookingSystem.Service.Service
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _config;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IImageRepository _imageRepository;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IConfiguration config, IMapper mapper, IImageRepository imageRepository)
        {
            _config = config;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _mapper = mapper;
            _imageRepository = imageRepository;
        }

        public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            User user = await _userRepository.GetUserById(userId);
            if (await Login(user.Email, oldPassword) is null)
                return false;
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            _userRepository.Update(user);
            return await _userRepository.SaveChange();
        }

        public async Task<CreateAccountResponse> CreateAccount(CreateAccountRequest request)
        {
            if (string.IsNullOrEmpty(request.FullName)  || string.IsNullOrEmpty(request.Email) 
                || string.IsNullOrEmpty(request.PhoneNumber)
                || request.YearOfBirth.Year > DateTime.UtcNow.Year
                || DateTime.UtcNow.Year - request.YearOfBirth.Year > 120)
                return null;

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(request.Email, emailPattern))
                return null;

            string phoneNumberPattern = @"^0\d{9}$";
            if (!Regex.IsMatch(request.PhoneNumber, phoneNumberPattern))
                return null;

            if (DateTime.Now.Year - request.YearOfBirth.Year > 120 || request.YearOfBirth.Year - DateTime.Now.Year < 4)
                return null;

            if (await _userRepository.GetUserByEmail(request.Email) != null)
                return null;

            User user = new()
            {
                Role = (Role)request.Role,
                Email = request.Email,
                FullName = request.FullName,
                YearOfBirth = request.YearOfBirth,
                PhoneNumber = request.PhoneNumber,
                IsVerified = true,      
                VerifyToken = "",
                CategoryId = request.CategoryId,
            };

            string password = new Random().Next(11234500, 2131232312).ToString();

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            _userRepository.Create(user);

            if (await _userRepository.SaveChange())
            {                
                var result = _mapper.Map<CreateAccountResponse>(user);
                result.Password = password;
                return result;
            }
            return null;
        }

        public async Task<string> GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(_config["Jwt:Issuer"]
                    , _config["Jwt:Audience"]
                    , new Claim[]
                    {
                        new(ClaimTypes.Name, user.FullName),
                        new(ClaimTypes.Role, user.Role.ToString()),
                        new(ClaimTypes.Email, user.Email),
                        new(ClaimTypes.MobilePhone, user.PhoneNumber),
                    },
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: credentials
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        public async Task<string> GenerateToken(User user, int extraDayExpired)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(_config["Jwt:Issuer"]
                    , _config["Jwt:Audience"]
                    , new Claim[]
                    {
                        new(ClaimTypes.Name, user.FullName),
                        new(ClaimTypes.Role, user.Role.ToString()),
                        new(ClaimTypes.Email, user.Email),
                    },
                    expires: DateTime.Now.AddDays(extraDayExpired),
                    signingCredentials: credentials
                );
            
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        public async Task<List<UserResponse>> GetCustomers() =>
            _mapper.Map<List<UserResponse>>(await _userRepository.GetCustomers());

        public async Task<List<SkinTherapistResponse>> GetSkinTherapists() =>
            _mapper.Map<List<SkinTherapistResponse>>(await _userRepository.GetSkinTherapists());

        public async Task<List<UserResponse>> GetStaffs() =>
            _mapper.Map<List<UserResponse>>(await _userRepository.GetStaffs());

        public async Task<List<ViewUser>> GetUsers() =>
            _mapper.Map<List<ViewUser>>(await _userRepository.GetUsers());

        public async Task<User> Login(string email, string password)
        {
            User user = await _userRepository.GetUserByEmail(email);
            if (user is null || !user.IsVerified 
                || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Success)
                return null;
            return user;
        }

        public async Task<bool> Register(Role role, string email, string password, string fullName, DateTime YearOfBirth, string PhoneNumber)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(password)
                || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(PhoneNumber)
                || YearOfBirth.Year > DateTime.UtcNow.Year
                || DateTime.UtcNow.Year - YearOfBirth.Year > 120)
                return false;

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(email, emailPattern))
                return false;

            string phoneNumberPattern = @"^0\d{9}$";
            if (!Regex.IsMatch(PhoneNumber, phoneNumberPattern))
                return false;

            if (DateTime.Now.Year - YearOfBirth.Year  > 120 || YearOfBirth.Year - DateTime.Now.Year < 4)
                return false;

            if (await _userRepository.GetUserByEmail(email) != null) 
                return false;

            User user = new()
            {
                Role = role,
                Email = email,
                FullName = fullName,
                YearOfBirth = YearOfBirth,
                PhoneNumber = PhoneNumber,
                IsVerified = false,
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            user.VerifyToken = await GenerateToken(user, 3);

            _userRepository.Create(user);

            if (await _userRepository.SaveChange())
            {
                SendEmail(user.Email, user.FullName, user.VerifyToken);
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveUser(RemoveUserRequest request)
        {
            User user = await _userRepository.GetUserById(request.UserId);
            if (user == null) return false;
            user.IsDeleted = true;
            _userRepository.Update(user);
            return await _userRepository.SaveChange();
        }

        public async Task<bool> ResetPassword(string email)
        {
            User user = await _userRepository.GetUserByEmail(email);
            if (user is null)
                return false;
            int newPassword = new Random().Next(10000000, 99999999);
            
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword.ToString());

            if (await _userRepository.SaveChange())
            {
                SendEmail(email, user.FullName, newPassword);
                return true;
            }            
            return false;
        }

        public async void SendEmail(string customerEmail, string userName, string token)
        {
            // Sender's email and password (use App Password if 2FA is enabled)
            string senderEmail = "mysteam666phk@gmail.com";
            string senderPassword = "jbtxpjiqkhzfalzb";

            // Receiver's email
            string receiverEmail = customerEmail;
            string htmlTemplate = @"
<!DOCTYPE html>
<html lang=""vi"">
<head>
  <meta charset=""UTF-8"">
  <title>Xác nhận đăng ký</title>
  <style type=""text/css"">
    body {
      font-family: Arial, sans-serif;
      background-color: #f4f4f4;
      margin: 0;
      padding: 0;
    }
    .container {
      background-color: #ffffff;
      width: 100%;
      max-width: 600px;
      margin: 0 auto;
      padding: 20px;
      border: 1px solid #dddddd;
    }
    .header {
      text-align: center;
      padding-bottom: 20px;
      border-bottom: 1px solid #dddddd;
    }
    .header h1 {
      margin: 0;
      color: #f9b37a;
    }
    .content {
      padding: 20px 0;
      line-height: 1.5;
      color: #333333;
    }
    .content p {
      margin: 10px 0;
    }
    .btn {
      display: inline-block;
      padding: 10px 20px;
      background-color: #f9b37a;
      color: #ffffff;
      text-decoration: none;
      border-radius: 4px;
      margin-top: 20px;
    }
    .footer {
      text-align: center;
      font-size: 12px;
      color: #777777;
      border-top: 1px solid #dddddd;
      padding-top: 10px;
    }
  </style>
</head>
<body>
  <div class=""container"">
    <!-- Phần header -->
    <div class=""header"">
      <h1>Skincare Booking System</h1>
    </div>
    <!-- Phần nội dung chính -->
    <div class=""content"">
      <p>Chào {0},</p>
      <p>Cảm ơn bạn đã đăng ký sử dụng dịch vụ của chúng tôi.</p>
      <p>Để hoàn tất việc đăng ký, vui lòng nhấn vào nút xác nhận bên dưới:</p>
      <p style=""text-align: center;"">
        <a href=""{1}"" class=""btn"">Xác nhận Email</a>
      </p>
      <p>Nếu bạn không thực hiện đăng ký, hãy bỏ qua email này.</p>
      <p>Trân trọng,<br>Đội ngũ Skincare Booking System</p>
    </div>
    <!-- Phần footer -->
    <div class=""footer"">
      <p>&copy; 2025 Skincare Booking System. All rights reserved.</p>
    </div>
  </div>
</body>
</html>";
            string verifyUrl = "https://localhost:7101/api/User/Verify?token=" + token;
            string emailBody = htmlTemplate.Replace("{0}", "Khiem");
            emailBody = emailBody.Replace("{1}", verifyUrl);
            //Get error when using string.Format

            // Setup the email message
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(senderEmail);
            mail.To.Add(receiverEmail);
            mail.IsBodyHtml = true;
            mail.Subject = "Verify Account Email";
            mail.Body = emailBody;

            // Configure SMTP client
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtp.EnableSsl = true;

            // Send the email
            smtp.Send(mail);
            Console.WriteLine("Email sent successfully!");
        }

        public async void SendEmail(string customerEmail, string userName, int newPassword)
        {
            // Sender's email and password (use App Password if 2FA is enabled)
            string senderEmail = "mysteam666phk@gmail.com";
            string senderPassword = "jbtxpjiqkhzfalzb";

            // Receiver's email
            string receiverEmail = customerEmail;
            string htmlTemplate = @"
<!DOCTYPE html>
<html lang=""vi"">
<head>
  <meta charset=""UTF-8"">
  <title>Xác nhận đăng ký</title>
  <style type=""text/css"">
    body {
      font-family: Arial, sans-serif;
      background-color: #f4f4f4;
      margin: 0;
      padding: 0;
    }
    .container {
      background-color: #ffffff;
      width: 100%;
      max-width: 600px;
      margin: 0 auto;
      padding: 20px;
      border: 1px solid #dddddd;
    }
    .header {
      text-align: center;
      padding-bottom: 20px;
      border-bottom: 1px solid #dddddd;
    }
    .header h1 {
      margin: 0;
      color: #f9b37a;
    }
    .content {
      padding: 20px 0;
      line-height: 1.5;
      color: #333333;
    }
    .content p {
      margin: 10px 0;
    }
    .btn {
      display: inline-block;
      padding: 10px 20px;
      background-color: #f9b37a;
      color: #ffffff;
      text-decoration: none;
      border-radius: 4px;
      margin-top: 20px;
    }
    .footer {
      text-align: center;
      font-size: 12px;
      color: #777777;
      border-top: 1px solid #dddddd;
      padding-top: 10px;
    }
  </style>
</head>
<body>
  <div class=""container"">
    <!-- Phần header -->
    <div class=""header"">
      <h1>Skincare Booking System</h1>
    </div>
    <!-- Phần nội dung chính -->
    <div class=""content"">
      <p>Chào {0},</p>
      <p>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi.</p>
      <p>Mật khẩu mới của tài khoảng bạn là:</p>
      <h1> {1} </h1>
      <p>Trân trọng,<br>Đội ngũ Skincare Booking System</p>
    </div>
    <!-- Phần footer -->
    <div class=""footer"">
      <p>&copy; 2025 Skincare Booking System. All rights reserved.</p>
    </div>
  </div>
</body>
</html>";
            string emailBody = htmlTemplate.Replace("{0}", "Khiem");
            emailBody = emailBody.Replace("{1}", newPassword.ToString());
            //Get error when using string.Format

            // Setup the email message
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(senderEmail);
            mail.To.Add(receiverEmail);
            mail.IsBodyHtml = true;
            mail.Subject = "Verify Account Email";
            mail.Body = emailBody;

            // Configure SMTP client
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtp.EnableSsl = true;

            // Send the email
            smtp.Send(mail);
            Console.WriteLine("Email sent successfully!");
        }

        public async Task<bool> UpdateRole(int userId, Role role)
        {
            User user = await _userRepository.GetUserById(userId);
            if (user is null)
                return false;
            user.Role = role;
            _userRepository.Update(user);
            return await _userRepository.SaveChange();
        }

        public async Task<bool> UpdateUserDescription(UpdateUserDescriptionRequest request)
        {
            User user = await _userRepository.GetUserById(request.UserId);
            if (user is null || user.Role != (Role)3) return false;
            user.Description = request.Description;
            _userRepository.Update(user);
            return await _userRepository.SaveChange();
        }

        public async Task<bool> UploadAvatarForUser(UploadAvatarForUserRequest request)
        {
            User user = await _userRepository.GetUserById(request.UserId);
            if (user is null) return false;
            Image image = await _imageRepository.GetImageById(request.ImageId);
            if (image is null) return false;    
            user.Image = image;
            _userRepository.Update(user);
            return await _userRepository.SaveChange();
        }

        public async Task<bool> VerifyAccount(string token)
        {
            if (string.IsNullOrEmpty(token)) 
                return false;

            JwtSecurityToken tokenSecurityToken = new JwtSecurityTokenHandler()
                .ReadJwtToken(token);

            if (tokenSecurityToken is null)
                return false;

            var expUnixTime = tokenSecurityToken.Payload.Exp;

            if(expUnixTime is null)
                return false;

            DateTime expDateTime = DateTimeOffset.FromUnixTimeSeconds(expUnixTime.Value).UtcDateTime;

            if (expDateTime < DateTime.UtcNow)
                return false;

            string email = tokenSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
            var role = tokenSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            string fullName = tokenSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            User user = await _userRepository.GetUserByNameByEmailByRole(email, (Role)Enum.Parse(typeof(Role), role), fullName);

            if (user is null) 
                return false;

            user.IsVerified = true;

            _userRepository.Update(user);

            return await _userRepository.SaveChange();
        }
    }
}
