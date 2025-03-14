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
        public Task<List<SkincareService>> GetServices();
        public Task<SkincareService> GetServiceByid(int serviceId);
        public Task<SkincareService> GetServiceByname(string serviceName);
        public Task<List<SkincareService>> Search(string search);
        public Task<bool> Delete(int id);
        public Task<bool> Update(int id, string serviceName, decimal price, DateTime workTime);
        public Task<bool> Create(string serviceName, decimal price, DateTime workTime);
        public Task<Dictionary<int, List<SkincareService>>> GetRandomServicesByCategory(int count);
    }
}
