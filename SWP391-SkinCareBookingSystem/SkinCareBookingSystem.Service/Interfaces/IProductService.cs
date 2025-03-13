using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface IProductService
    {
        public Task<List<Product>> Search(string productName);
        public Task<List<Product>> Search(Decimal minPrice, Decimal maxPrice);
        public Task<List<Product>> Search(Decimal underPrice);        
        public Task<List<Product>> Search(int categoryId);        
        public Task<List<Product>> SearchDescPrice();        
        public Task<List<Product>> SearchAscPrice();       
        public Task<bool> AddProducts(List<Product> products);
        public Task<bool> RemoveProduct(int productId);
    }
}
