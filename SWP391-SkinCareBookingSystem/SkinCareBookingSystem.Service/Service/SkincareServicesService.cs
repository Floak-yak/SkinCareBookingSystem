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
        private readonly IImageRepository _imageRepository;
        private readonly ICategoryRepository _categoryRepository;

        public SkincareServicesService(
            ISkincareServiceRepository skincareServiceRepository, 
            IImageRepository imageRepository,
            ICategoryRepository categoryRepository)
        {
            _skincareServicesRepository = skincareServiceRepository;
            _imageRepository = imageRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<bool> Create(string serviceName, string serviceDescription, decimal price, int workTime, int categoryId, int? imageId, string benefits)
        {
            try
            {
                // Log input values
                Console.WriteLine($"Creating service with: Name={serviceName}, Desc={serviceDescription}, Price={price}, WorkTime={workTime}, CategoryId={categoryId}, ImageId={imageId}");
                
                // Additional validation
                if (string.IsNullOrEmpty(serviceName) || string.IsNullOrEmpty(serviceDescription))
                {
                    Console.WriteLine("ServiceName or ServiceDescription is null or empty");
                    return false;
                }
                
                serviceName = serviceName.Trim();
                serviceDescription = serviceDescription.Trim();
                
                if (serviceName.Length > 100)
                {
                    Console.WriteLine("ServiceName exceeds 100 characters");
                    return false;
                }
                
                if (serviceDescription.Length > 500)
                {
                    Console.WriteLine("ServiceDescription exceeds 500 characters");
                    return false;
                }
                
                if (price <= 0)
                {
                    Console.WriteLine("Price must be greater than 0");
                    return false;
                }
                
                if (workTime < 1 || workTime > 90)
                {
                    Console.WriteLine("WorkTime must be between 1 and 90 minutes");
                    return false;
                }
                
                if (categoryId <= 0)
                {
                    Console.WriteLine("CategoryId must be greater than 0");
                    return false;
                }
                
                if (await _skincareServicesRepository.IsServiceExist(serviceName))
                {
                    Console.WriteLine("Service with this name already exists");
                    return false;
                }
                
                // Validate that the image exists if an ID is provided
                if (imageId.HasValue)
                {
                    var image = await _imageRepository.GetImageById(imageId.Value);
                    if (image == null)
                    {
                        Console.WriteLine($"Image with ID {imageId.Value} not found");
                        return false;
                    }
                }
                
                SkincareService skincareServices = new()
                {
                    ServiceName = serviceName.Trim(),
                    ServiceDescription = serviceDescription.Trim(),
                    Price = price,
                    WorkTime = workTime,
                    CategoryId = categoryId,
                    ImageId = imageId ?? 1,
                    Benefits = benefits.Trim()
                };

                _skincareServicesRepository.Create(skincareServices);
                var saveResult = await _skincareServicesRepository.SaveChange();
                if (!saveResult)
                {
                    Console.WriteLine("Failed to save changes to database");
                }
                return saveResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating service: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
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
                var services = await _skincareServicesRepository.GetServices();
                return services.ToList();
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
                    return await GetServices();
                    
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

        public async Task<bool> Update(int id, string serviceName, string serviceDescription, decimal? price, int? workTime, int? categoryId, int? imageId, string benefits)
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
                    if (workTime.Value < 1 || workTime.Value > 90)
                        return false;
                    skincareService.WorkTime = workTime.Value;
                }

                if (categoryId.HasValue)
                {
                    if (categoryId.Value <= 0)
                        return false;
                    skincareService.CategoryId = categoryId.Value;
                }

                if (imageId.HasValue)
                {
                    skincareService.ImageId = imageId.Value;
                }

                if (benefits != null)
                {
                    skincareService.Benefits = benefits.Trim();
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
