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
        public async Task<bool> Create(string serviceName, decimal price, DateTime workTime)
        {
            try
            {
                if (string.IsNullOrEmpty(serviceName) || serviceName.Length > 100)
                    return false;
                if (price <= 0 || price > 1000000)
                    return false;

                if (workTime == DateTime.MinValue || workTime.TimeOfDay.TotalHours > 3.5)
                    return false;

                if (await _skincareServicesRepository.IsServiceExist(serviceName))
                    return false;
                
                SkincareService skincareServices = new()
                {
                    ServiceName = serviceName.Trim(),
                    Price = price,
                    WorkTime = workTime
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

        public async Task<List<SkincareService>> GetServices()
        {
            try
            {
                return await _skincareServicesRepository.GetServices();
            }
            catch (Exception)
            {
                return new List<SkincareService>();
            }
        }

        public async Task<List<SkincareService>> Search(string search)
        {
            try
            {
                if (string.IsNullOrEmpty(search)) 
                    return await GetServices();
                return await _skincareServicesRepository.Search(search);
            }
            catch (Exception)
            {
                return new List<SkincareService>();
            }
        }

        public async Task<bool> Update(int id, string serviceName, decimal price, DateTime workTime)
        {
            try
            {
                SkincareService skincareService = await _skincareServicesRepository.GetServiceById(id);
                if (skincareService == null) 
                    return false;
                if (!string.IsNullOrEmpty(serviceName))
                {
                    if(await _skincareServicesRepository.IsServiceExist(serviceName)) 
                        return false;
                    skincareService.ServiceName = serviceName;
                }
                if (price <= 0)
                    return false;
                skincareService.Price = price;

                // maximum service duration is 3.5 hours
                if (workTime == DateTime.MinValue || workTime.TimeOfDay.TotalHours > 3.5)
                    return false;
                skincareService.WorkTime = workTime;

                _skincareServicesRepository.Update(skincareService);
                return await _skincareServicesRepository.SaveChange();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
