using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Interfaces
{
    public interface ISkincareServiceRepository
    {
        public Task<List<SkincareService>> GetServices();
        public Task<SkincareService> GetServiceById(int serviceId);
        public Task<SkincareService> GetServiceByname(string name);
        public Task<List<SkincareService>> Search(string search);
        public Task<List<SkincareService>> GetSkincareServicesBySkinTherapistId(int skinTherapistId);
        public Task<bool> Delete(int id);
        public void Update(SkincareService service);
        public void Create(SkincareService service);
        public Task<bool> SaveChange();
        public Task<bool> IsServiceExist(string serviceName);
        public Task<Dictionary<int, List<SkincareService>>> GetRandomServicesByCategory(int count);
    }
}
