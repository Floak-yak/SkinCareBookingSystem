using Microsoft.EntityFrameworkCore.Diagnostics;
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
    public class SkincareServicesService : ISkincareServicesService
    {
        private readonly ISkincareServiceRepository _skincareServicesRepository;

        public SkincareServicesService(ISkincareServiceRepository skincareServiceRepository)
        {
            _skincareServicesRepository = skincareServiceRepository;
        }
        public async Task<bool> Create(string serviceName, string serviceDescription, decimal price, DateTime workTime, int categoryId, int? imageId)
        {
            try
            {
                if (string.IsNullOrEmpty(serviceName) || serviceName.Length > 100)
                    return false;
                if (string.IsNullOrEmpty(serviceDescription) || serviceDescription.Length > 500)
                    return false;
                if (price <= 0)
                    return false;
                if (workTime == DateTime.MinValue || workTime.TimeOfDay.TotalHours > 3.5)
                    return false;
                if (categoryId <= 0)
                    return false;

                if (await _skincareServicesRepository.IsServiceExist(serviceName))
                    return false;
                
                SkincareService skincareServices = new()
                {
                    ServiceName = serviceName.Trim(),
                    ServiceDescription = serviceDescription.Trim(),
                    Price = price,
                    WorkTime = (int)workTime.TimeOfDay.TotalMinutes,
                    CategoryId = categoryId,
                };

                _skincareServicesRepository.Create(skincareServices);
                return await _skincareServicesRepository.SaveChange();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                SkincareService service = await _skincareServicesRepository.GetServiceById(id);
                if (service == null)
                    return false;
                
                if (service.BookingServiceSchedules != null && service.BookingServiceSchedules.Any())
                    return false;

                return await _skincareServicesRepository.Delete(id);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<SkincareService> GetServiceByid(int serviceId)
        {
            try
            {
                return await _skincareServicesRepository.GetServiceById(serviceId);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<SkincareService> GetServiceByname(string name)
        {
            try
            {
                return await _skincareServicesRepository.GetServiceByname(name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<SkincareService>> GetServices(int page = 1, int pageSize = 10)
        {
            try
            {
                var services = await _skincareServicesRepository.GetServices();
                return services
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<SkincareService>();
            }
        }

        public async Task<List<SkincareService>> Search(string search, int page = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(search)) 
                    return await GetServices(page, pageSize);
                    
                var services = await _skincareServicesRepository.Search(search);
                return services
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
            }
            catch (Exception)
            {
                return new List<SkincareService>();
            }
        }

        public async Task<bool> Update(int id, string serviceName, string serviceDescription, decimal? price, DateTime? workTime, int? categoryId, int? imageId)
        {
            try
            {
                SkincareService skincareService = await _skincareServicesRepository.GetServiceById(id);
                if (skincareService == null) 
                    return false;

                if (!string.IsNullOrEmpty(serviceName))
                {
                    serviceName = serviceName.Trim();
                    if (serviceName.Length > 100)
                        return false;
                    
                    var existingService = await _skincareServicesRepository.GetServiceByname(serviceName);
                    if (existingService != null && existingService.Id != id)
                        return false;
                    
                    skincareService.ServiceName = serviceName;
                }

                if (!string.IsNullOrEmpty(serviceDescription))
                {
                    serviceDescription = serviceDescription.Trim();
                    if (serviceDescription.Length > 500)
                        return false;
                    
                    skincareService.ServiceDescription = serviceDescription;
                }

                if (price.HasValue)
                {
                    if (price.Value <= 0)
                        return false;
                    skincareService.Price = price.Value;
                }

                if (workTime.HasValue)
                {
                    if (workTime.Value == DateTime.MinValue || workTime.Value.TimeOfDay.TotalHours > 3.5)
                        return false;
                    skincareService.WorkTime = (int)workTime.Value.TimeOfDay.TotalMinutes;
                }

                if (categoryId.HasValue)
                {
                    if (categoryId.Value <= 0)
                        return false;
                    skincareService.CategoryId = categoryId.Value;
                }

                _skincareServicesRepository.Update(skincareService);
                return await _skincareServicesRepository.SaveChange();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Dictionary<int, List<SkincareService>>> GetRandomServicesByCategory(int count)
        {
            try
            {
                if (count <= 0)
                    return new Dictionary<int, List<SkincareService>>();

                return await _skincareServicesRepository.GetRandomServicesByCategory(count);
            }
            catch (Exception)
            {
                return new Dictionary<int, List<SkincareService>>();
            }
        }
    }
}
