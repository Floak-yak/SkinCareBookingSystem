﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Repositories.Repositories;
using SkinCareBookingSystem.Service.Dto.ImageDto;
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
        private readonly IMapper _mapper;
        private readonly ITransactionRepository _transactionRepository;

        public ImageService(IImageRepository imageRepository, IMapper mapper, ITransactionRepository transactionRepository)
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
            _transactionRepository = transactionRepository;
        }

        public async Task<StoreImageResponse> StoreImage(IFormFile imageRequest, string? description)
        {
            using var memoryStream = new MemoryStream();
            await imageRequest.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();
            if (string.IsNullOrEmpty(description))
            {
                description = imageRequest.FileName;
            }
            Image image = new()
            {
                Bytes = fileBytes,
                Size = Math.Round(imageRequest.Length / 1024m, 2),
                Description = description,
                FileExtension = Path.GetExtension(imageRequest.FileName)
            };

            _imageRepository.CreateImage(image);
            if (await _imageRepository.SaveChange())
            {
                return _mapper.Map<StoreImageResponse>(image);
            }
            return null;
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

        public async Task<Image> GetImageById(int imageId) =>
            await _imageRepository.GetImageById(imageId);

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

        public async Task<List<Image>> GetImages() =>
            await _imageRepository.GetAllImages();

        public async Task<Image> AddImage(string imageLink)
        {
            if (string.IsNullOrEmpty(imageLink) ||
                string.IsNullOrEmpty(ConvertImageToBase64(imageLink)))
                throw new ArgumentNullException(nameof(imageLink));

            byte[] imageBytes = File.ReadAllBytes(imageLink);
            string extension = Path.GetExtension(imageLink);
            decimal size = imageBytes.Length / 1024m; // Convert bytes to KB

            return new Image
            {
                Id = 0, // DB will assign this
                Bytes = imageBytes,
                Description = $"Local Image from {imageLink}",
                FileExtension = extension,
                Size = size
            };
        }

        public async Task<bool> UploadImage(int bookingId, IFormFile image)
        {
            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();
            Image imageStore = new()
            {
                Bytes = fileBytes,
                Size = Math.Round(image.Length / 1024m, 2),
                Description = DateTime.Now.ToString(),
                FileExtension = Path.GetExtension(image.FileName)
            };
            _imageRepository.CreateImage(imageStore);
            Transaction transaction = await _transactionRepository.GetTransactionByBookingId(bookingId);
            if (transaction is null)
                return false;
            transaction.Image = imageStore;
            return await _imageRepository.SaveChange();
        }
    }
}
