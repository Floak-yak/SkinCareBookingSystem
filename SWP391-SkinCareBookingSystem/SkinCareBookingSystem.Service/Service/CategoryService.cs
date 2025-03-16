using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SkinCareBookingSystem.Service.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IConfiguration _config;

        public CategoryService(ICategoryRepository categoryRepository, IConfiguration config)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<List<Category>> GetCategories()
        {
            try
            {
                return await _categoryRepository.GetCategory();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CategoryService.GetCategories: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<Category> GetCategoryById(int categoryId)
        {
            return await _categoryRepository.GetCategoryById(categoryId);
        }

        public async Task<bool> CreateCategoryUserId(string categoryName, int userId)
        {
            if (string.IsNullOrEmpty(categoryName) || userId <= 0)
                return false;

            if (!Regex.IsMatch(categoryName, @"^[a-zA-Z0-9\s]+$"))
                return false;

            if (await _categoryRepository.IsCategoryExist(categoryName))
                return false;

            Category category = new()
            {
                CategoryName = categoryName,
            };

            _categoryRepository.Create(category);
            return await _categoryRepository.Savechange();
        }

        public async Task<bool> CreateCategory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                return false;

            if (!Regex.IsMatch(categoryName, @"^[a-zA-Z0-9\s]+$"))
                return false;

            if (await _categoryRepository.IsCategoryExist(categoryName))
                return false;

            Category category = new()
            {
                CategoryName = categoryName,
            };

            _categoryRepository.Create(category);
            return await _categoryRepository.Savechange();
        }

        public async Task<bool> UpdateCategory(int categoryId, string newCategoryName)
        {
            Category category = await _categoryRepository.GetCategoryById(categoryId);
            if (category == null)
                return false;

            if (await _categoryRepository.IsCategoryExist(newCategoryName))
                return false;

            category.CategoryName = newCategoryName;
            _categoryRepository.Update(category);
            return await _categoryRepository.Savechange();
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            return await _categoryRepository.Delete(categoryId);
        }

        public async Task<Category> GetCategoryByName(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                return null;
            return await _categoryRepository.GetCategoryByName(categoryName);
        }

        public async Task<bool> IsCategoryExist(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                return false;
            return await _categoryRepository.IsCategoryExist(categoryName);
        }
    }
}
