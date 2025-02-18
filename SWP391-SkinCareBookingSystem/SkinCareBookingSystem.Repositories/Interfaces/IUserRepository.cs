using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<User>> GetUsers();
        public Task<User> GetUserByEmail(string email);
        public Task<User> GetUserById(int userId);
        public Task<User> GetUserByName(string userName);
        public void Create(User user);
        public void Update(User user);
        public Task<bool> Delete(int userId);
        public Task<bool> SaveChange();
        public Task<bool> IsUserExist(string userName);
        public Task<User> GetUserByNameByEmailByRole(string email, Role role, string userName);
    }
}
