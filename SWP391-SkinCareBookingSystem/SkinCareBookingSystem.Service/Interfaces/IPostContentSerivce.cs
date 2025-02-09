using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface IPostContentSerivce
    {
        public Task<Content> GetContentByIdAsync(int contentId);
        public Task<List<Content>> GetContentsAsync();
        public Task<bool> DeleteContent(int contentId);
        public Task<bool> CreateContent(string contentOfPost, ContentType contentType, int position, string imageLink, int postId);
        public Task<bool> UpdateContent(int contentId, string contentOfPost, ContentType contentType, int position, string imageLink);
    }
}
