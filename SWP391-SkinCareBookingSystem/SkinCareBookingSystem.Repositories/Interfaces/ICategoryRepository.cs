using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<List<Category>> GetCategory();
        public Task<Category> GetCategoryById(int categoryId);
        public Task<Category> GetCategoryByName(string categoryName);
        public void Create(Category category);
        public void Update(Category category);
        public Task<bool> Delete(int id);
        public Task<bool> Savechange();
        public Task<bool> IsCategoryExist(string categoryName);
    }
}
