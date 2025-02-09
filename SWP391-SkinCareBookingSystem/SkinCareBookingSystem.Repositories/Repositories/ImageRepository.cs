using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Data;
using SkinCareBookingSystem.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext _context;

        public ImageRepository(AppDbContext appDbContext) => _context = appDbContext; 
        public void CreateImage(Image image)
        {
            _context.Images.Add(image);
        }

        public async Task<bool> DeleteImage(int imageId)
        {
            Image image = await GetImageById(imageId);
            if (image is null)
                return false;
            _context.Remove(image);
            return await SaveChange();
        }

        public async Task<Image> GetImageByDescription(string description) =>
            await _context.Images
                .FirstOrDefaultAsync(i => i.Description.Equals(description));

        public async Task<Image> GetImageById(int imageId) => await _context.Images.FirstOrDefaultAsync(i => i.Id == imageId);
        

        public async Task<Image> GetImageByPostContentId(int postContentId) =>
            await _context.Images.FirstOrDefaultAsync(i => i.Content.Id == postContentId);

        public async Task<Image> GetImageByPostId(int postId) =>
            await _context.Images.FirstOrDefaultAsync(i => i.Post.Id == postId);


        public async Task<Image> GetImageByProductId(int productId) =>
            await _context.Images.FirstOrDefaultAsync(i => i.Product.Id == productId);

        public async Task<bool> SaveChange()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void UpdateImage(Image image)
        {
            _context.Images.Update(image);
        }
    }
}
