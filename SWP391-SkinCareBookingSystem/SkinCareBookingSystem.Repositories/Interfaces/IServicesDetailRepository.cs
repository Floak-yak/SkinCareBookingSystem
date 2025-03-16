using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Interfaces
{
    public interface IServicesDetailRepository
    {
        Task<IEnumerable<ServicesDetail>> GetAllServicesDetailsAsync();
        Task<ServicesDetail> GetServicesDetailByIdAsync(int id);
        Task<ServicesDetail> CreateServicesDetailAsync(ServicesDetail servicesDetail);
        Task<ServicesDetail> UpdateServicesDetailAsync(ServicesDetail servicesDetail);
        Task DeleteServicesDetailAsync(int id);
        Task<IEnumerable<ServicesDetail>> GetServicesDetailsByServiceIdAsync(int serviceId);
    }
}