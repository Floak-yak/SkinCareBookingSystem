using SkinCareBookingSystem.BusinessObject.Entity;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface ISkincareServicesService
    {
        public Task<List<SkincareService>> GetServices(int page = 1, int pageSize = 10);
        public Task<SkincareService> GetServiceByid(int serviceId);
        public Task<SkincareService> GetServiceByname(string serviceName);
        public Task<List<SkincareService>> Search(string search, int page = 1, int pageSize = 10);
        public Task<bool> Delete(int id);
        public Task<bool> Update(int id, string serviceName, string serviceDescription, decimal? price, int? workTime, int? categoryId, int? imageId);
        public Task<bool> Create(string serviceName, string serviceDescription, decimal price, int workTime, int categoryId, int? imageId);
        public Task<Dictionary<int, List<SkincareService>>> GetRandomServicesByCategory(int count);
    }
}
