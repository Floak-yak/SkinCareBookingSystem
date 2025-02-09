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
            if(string.IsNullOrEmpty(serviceName) || workTime == DateTime.MinValue || price <= 0)
                return false;
            if(await _skincareServicesRepository.IsServiceExist(serviceName)) 
                return false;
            
            SkincareService skincareServices = new()
            {
                ServiceName = serviceName,
                Price = price,
                WorkTime = workTime
            };
            _skincareServicesRepository.Create(skincareServices);
            return await _skincareServicesRepository.SaveChange();
        }

        public async Task<bool> Delete(int id)
        {
            return await _skincareServicesRepository.Delete(id);
        }

        public async Task<SkincareService> GetServiceByid(int serviceId)
        {
            return await _skincareServicesRepository.GetServiceById(serviceId);
        }

        public async Task<SkincareService> GetServiceByname(string name)
        {
            return await _skincareServicesRepository.GetServiceByname(name);
        }


        public Task<List<SkincareService>> GetServices()
        {
            return _skincareServicesRepository.GetServices();
        }

        public async Task<List<SkincareService>> Search(string search)
        {
            if (string.IsNullOrEmpty(search)) return null;
            return await _skincareServicesRepository.Search(search);
        }

        public async Task<bool> Update(int id, string serviceName, decimal price, DateTime workTime)
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

            if (workTime == DateTime.MinValue || workTime > new DateTime().AddHours(3).AddMinutes(30))
                return false;
            skincareService.WorkTime = workTime;

            _skincareServicesRepository.Update(skincareService);
            return await _skincareServicesRepository.SaveChange();
        }
    }
}
