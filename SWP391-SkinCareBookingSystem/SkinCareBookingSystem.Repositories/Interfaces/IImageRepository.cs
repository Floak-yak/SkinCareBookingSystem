using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Interfaces
{
    public interface IImageRepository
    {
        //public Task<Image> GetImageByPostId(int postId);
        //public Task<Image> GetImageByPostContentId(int postContentId);
        //public Task<Image> GetImageByProductId(int productId);
        public Task<Image> GetImageById(int imageId);   
        public Task<bool> DeleteImage(int imageId);
        public Task<Image> GetImageByDescription(string description);
        public void UpdateImage(Image image);
        public void CreateImage(Image image);
        public Task<bool> SaveChange();
        public Task<List<Image>> GetAllImages();
    }
}
