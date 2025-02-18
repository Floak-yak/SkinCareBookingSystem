using SkinCareBookingSystem.BusinessObject.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategories();
        Task<Category> GetCategoryById(int categoryId);
        Task<Category> GetCategoryByName(string categoryName);
        Task<bool> CreateCategory(string categoryName, int userId);
        Task<bool> UpdateCategory(int categoryId, string newCategoryName);
        Task<bool> DeleteCategory(int categoryId);
        Task<bool> IsCategoryExist(string categoryName);
        Task<bool> NotifyCategoryChange(string email, string categoryName);
    }
}
