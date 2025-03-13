using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Repositories.Repositories;
using SkinCareBookingSystem.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Service
{
	public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

		public async Task<bool> AddProducts(List<Product> products)
		{
            if (products.Count == 0)
                return false;
			if (products.Count == 1)
            {
                _productRepository.CreateProduct(products[0]);
                return await _productRepository.SaveChange();
            }
			_productRepository.CreateProducts(products);
			return await _productRepository.SaveChange();
		}

		public async Task<bool> RemoveProduct(int productId)
		{
			Product product = await _productRepository.GetProductById(productId);
            if (product == null) return false;
            return await _productRepository.RemoveProduct(product);
		}

		public async Task<List<Product>> Search(string productName) =>
            await _productRepository.Search(productName);

        public async Task<List<Product>> Search(decimal minPrice, decimal maxPrice) =>
            await _productRepository.Search(minPrice, maxPrice);

        public async Task<List<Product>> Search(decimal underPrice) =>
            await _productRepository.Search(underPrice);

        public async Task<List<Product>> Search(int categoryId) =>
            await _productRepository.Search(categoryId);

        public async Task<List<Product>> SearchAscPrice() =>
            await _productRepository.SearchAscPrice();

        public async Task<List<Product>> SearchDescPrice() =>
            await _productRepository.SearchDescPrice();
    }
}
