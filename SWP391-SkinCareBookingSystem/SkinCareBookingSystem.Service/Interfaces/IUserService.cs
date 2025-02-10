using SkinCareBookingSystem.BusinessObject.Entity;
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
        Task<List<User>> GetUsers();    
        Task<bool> Register(Role role, string email, string password, string fullName, DateTime YearOfBirth, string PhoneNumber);
    }
}
