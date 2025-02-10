using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Repositories.Repositories;
using SkinCareBookingSystem.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<User>> GetUsers() =>
            await _userRepository.GetUsers();

        public async Task<User> Login(string email, string password)
        {
            User user = await _userRepository.GetUserByEmail(email);
            if (user is null || !user.Password.Equals(password))
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

            User user = new()
            {
                Role = role,
                Email = email,
                FullName = fullName,
                Password = password,
                YearOfBirth = YearOfBirth,  
                PhoneNumber = PhoneNumber,
            };

            _userRepository.Create(user);

            if (await _userRepository.SaveChange())
            {
                SendEmail(email);
                return true;
            }
            return false;
        }

        public void SendEmail(string customerEmail)
        {
            // Sender's email and password (use App Password if 2FA is enabled)
            string senderEmail = "your-email@gmail.com";
            string senderPassword = "your-app-password";

            // Receiver's email
            string receiverEmail = "receiver-email@example.com";

            // Setup the email message
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(senderEmail);
            mail.To.Add(receiverEmail);
            mail.IsBodyHtml = true;
            mail.Subject = "Test Email from C#";
            mail.Body = @"
                    <html>
                    <body style='font-family:Arial, sans-serif; color:#333;'>
                        <h2 style='color:#007BFF;'>Hello, This is a Test Email!</h2>
                        <p>Here is an <strong>HTML-formatted</strong> email sent from a <span style='color:green;'>C# program</span>.</p>
                        <hr>
                        <p style='font-size:14px;'>Best Regards,<br><strong>Your C# App</strong></p>
                    </body>
                    </html>";
            
            // Configure SMTP client
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtp.EnableSsl = true;

            // Send the email
            smtp.Send(mail);
            Console.WriteLine("Email sent successfully!");
        }
    }
}
