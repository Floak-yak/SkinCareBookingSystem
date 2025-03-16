using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Data;
using SkinCareBookingSystem.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public void Create(Category category)
        {
            _context.Categories.Add(category);
        }

        public async Task<bool> Delete(int id)
        {
            Category category = await GetCategoryById(id);
            if (category == null)
                return false;
            _context.Categories.Remove(category);
            return await Savechange();
        }

        public async Task<List<Category>> GetCategory()
        {
            try
            {
                if (_context.Categories == null)
                {
                    throw new InvalidOperationException("Categories DbSet is null");
                }

                return await _context.Categories
                    .AsNoTracking()
                    .Include(s => s.Posts)
                    .Include(s => s.Products)
                    .OrderBy(s => s.CategoryName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CategoryRepository.GetCategory: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task<Category> GetCategoryById(int categoryId)
        {
            return await _context.Categories
                .Include(s => s.Posts)
                .Include(s => s.Products)
                .OrderBy(s => s.CategoryName)
                .FirstOrDefaultAsync(s => s.Id == categoryId);
        }

        public async Task<Category> GetCategoryByName(string categoryName)
        {
            return await _context.Categories
                .Include(s => s.Posts)
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.CategoryName.Equals(categoryName));
        }

        public async Task<bool> IsCategoryExist(string categoryName)
        {
            return await GetCategoryByName(categoryName) != null;
        }

        public async Task<bool> Savechange()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }
    }
}
