using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface IImageService
    {
        public Task<bool> StoreImage(string imageLink);
        public Task<Image> GetImageByDescription(string description);
        public Task<Image> GetImageId(int imageId);
    }
}
