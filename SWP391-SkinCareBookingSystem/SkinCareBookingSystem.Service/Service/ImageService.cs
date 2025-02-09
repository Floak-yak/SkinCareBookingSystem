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
    public class ImageService : IImageService
    {
        private readonly IImageRepository _imageRepository;

        public ImageService(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        public async Task<bool> StoreImage(string imageLink)
        {
            if (string.IsNullOrEmpty(imageLink) || 
                string.IsNullOrEmpty(ConvertImageToBase64(imageLink))) 
                return false;

            Image image = new()
            {
                Bytes = File.ReadAllBytes(imageLink),
                Size = new FileInfo(imageLink).Length,
                Description = Path.GetFileName(imageLink),
                FileExtension = new FileInfo(imageLink).Extension
            };

            _imageRepository.CreateImage(image);
            return await _imageRepository.SaveChange();
        }

        public string ConvertImageToBase64(string imageLink)
        {
            try
            {
                byte[] imageBytes = File.ReadAllBytes(imageLink);
                return Convert.ToBase64String(imageBytes);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<Image> GetImageByDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
                return null;
            return await _imageRepository.GetImageByDescription(description);
        }

        public async Task<Image> GetImageId(int imageId) =>
            await _imageRepository.GetImageById(imageId);
    }
}
