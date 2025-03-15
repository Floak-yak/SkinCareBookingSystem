using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Service.Dto;
using SkinCareBookingSystem.Service.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface IUserService
    {
        Task<User> Login(string email, string password);
        Task<List<ViewUser>> GetUsers();
        Task<CreateAccountResponse> CreateAccount(Role role, string email, string fullName, DateTime YearOfBirth, string PhoneNumber);
        Task<bool> Register(Role role, string email, string password, string fullName, DateTime YearOfBirth, string PhoneNumber);
        Task<bool> VerifyAccount(string token);
        Task<bool> ResetPassword(string email);
        Task<bool> ChangePassword(int userId, string oldPassword, string newPassword);
        public Task<string> GenerateToken(User user);
        public Task<string> GenerateToken(User user, int extraDayExpired);
        public void SendEmail(string email, string userName, string token);
        public void SendEmail(string email, string userName, int newPassword);
        public Task<bool> UpdateRole(int userId ,Role role);
        public Task<List<UserResponse>> GetStaffs();
        public Task<List<UserResponse>> GetCustomers();
        public Task<List<SkinTherapistResponse>> GetSkinTherapists();
    }
}
