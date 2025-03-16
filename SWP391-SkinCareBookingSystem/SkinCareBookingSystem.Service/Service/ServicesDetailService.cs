using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Service
{
    public class ServicesDetailService : IServicesDetailService
    {
        private readonly IServicesDetailRepository _servicesDetailRepository;
        private readonly IImageRepository _imageRepository;

        public ServicesDetailService(IServicesDetailRepository servicesDetailRepository, IImageRepository imageRepository)
        {
            _servicesDetailRepository = servicesDetailRepository;
            _imageRepository = imageRepository;
        }

        public async Task<IEnumerable<ServicesDetail>> GetAllServicesDetailsAsync()
        {
            return await _servicesDetailRepository.GetAllServicesDetailsAsync();
        }

        public async Task<ServicesDetail> GetServicesDetailByIdAsync(int id)
        {
            return await _servicesDetailRepository.GetServicesDetailByIdAsync(id);
        }

        public async Task<ServicesDetail> CreateServicesDetailAsync(ServicesDetail servicesDetail)
        {
            if (string.IsNullOrWhiteSpace(servicesDetail.Title))
                throw new ArgumentException("Title is required");

            if (string.IsNullOrWhiteSpace(servicesDetail.Description))
                throw new ArgumentException("Description is required");

            if (servicesDetail.Duration <= 0)
                throw new ArgumentException("Duration must be greater than 0");

            // If ImageId is provided, verify the image exists
            if (servicesDetail.Image?.Id > 0)
            {
                var image = await _imageRepository.GetImageById(servicesDetail.Image.Id);
                if (image == null)
                    throw new ArgumentException($"Image with ID {servicesDetail.Image.Id} not found");
                servicesDetail.Image = image;
            }

            return await _servicesDetailRepository.CreateServicesDetailAsync(servicesDetail);
        }

        public async Task<ServicesDetail> UpdateServicesDetailAsync(ServicesDetail servicesDetail)
        {
            if (string.IsNullOrWhiteSpace(servicesDetail.Title))
                throw new ArgumentException("Title is required");

            if (string.IsNullOrWhiteSpace(servicesDetail.Description))
                throw new ArgumentException("Description is required");

            if (servicesDetail.Duration <= 0)
                throw new ArgumentException("Duration must be greater than 0");

            var existingService = await _servicesDetailRepository.GetServicesDetailByIdAsync(servicesDetail.Id);
            if (existingService == null)
                throw new ArgumentException($"Service detail with ID {servicesDetail.Id} not found");

            // If ImageId is provided, verify the image exists
            if (servicesDetail.Image?.Id > 0)
            {
                var image = await _imageRepository.GetImageById(servicesDetail.Image.Id);
                if (image == null)
                    throw new ArgumentException($"Image with ID {servicesDetail.Image.Id} not found");
                servicesDetail.Image = image;
            }

            return await _servicesDetailRepository.UpdateServicesDetailAsync(servicesDetail);
        }

        public async Task DeleteServicesDetailAsync(int id)
        {
            var existingService = await _servicesDetailRepository.GetServicesDetailByIdAsync(id);
            if (existingService == null)
                throw new ArgumentException($"Service detail with ID {id} not found");

            await _servicesDetailRepository.DeleteServicesDetailAsync(id);
        }

        public async Task<IEnumerable<ServicesDetail>> GetServicesDetailsByServiceIdAsync(int serviceId)
        {
            return await _servicesDetailRepository.GetServicesDetailsByServiceIdAsync(serviceId);
        }
    }
}