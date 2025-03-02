﻿using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using SkinCareBookingSystem.Service.Interfaces;

namespace SkinCareBookingSystem.Service.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IConfiguration _config;

        public CategoryService(ICategoryRepository categoryRepository, IConfiguration config)
        {
            _categoryRepository = categoryRepository;
            _config = config;
        }

        public async Task<List<Category>> GetCategories() =>
            await _categoryRepository.GetCategory();

        public async Task<Category> GetCategoryById(int categoryId)
        {
            return await _categoryRepository.GetCategoryById(categoryId);
        }

        public async Task<bool> CreateCategory(string categoryName, int userId)
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
                UserId = userId
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

        public Task<Category> GetCategoryByName(string categoryName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCategoryExist(string categoryName)
        {
            throw new NotImplementedException();
        }
    }
}
