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
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) => _context = context;
        public async Task<List<Product>> Search(string productName) => 
            await _context.Products
                .Where(p => p.ProductName.Contains(productName))
                .OrderBy(p => p.CreatedDate)
                .ToListAsync();

        public async Task<List<Product>> Search(decimal minPrice, decimal maxPrice) =>
            await _context.Products
                .Where(p => p.Price <= maxPrice && p.Price >= minPrice)
                .OrderBy(p => p.CreatedDate)
                .ToListAsync();

        public async Task<List<Product>> Search(decimal underPrice) =>
            await _context.Products
                .Where(p => p.Price <= underPrice)
                .OrderBy(p => p.CreatedDate)
                .ToListAsync();

        public async Task<List<Product>> Search(int categoryId) =>
            await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.CreatedDate)
                .ToListAsync();

        public async Task<List<Product>> SearchAscPrice() =>
            await _context.Products
                .OrderBy(p => p.Price)
                .ToListAsync();

        public async Task<List<Product>> SearchDescPrice() =>
            await _context.Products
                .OrderByDescending(p => p.Price)
                .ToListAsync();
    }
}
