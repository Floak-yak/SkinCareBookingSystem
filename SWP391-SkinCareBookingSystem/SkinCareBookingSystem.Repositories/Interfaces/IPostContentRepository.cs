using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Interfaces
{
    public interface IPostContentRepository
    {
        public Task<Content> GetContentByIdAsync(int contentId);
        public Task<List<Content>> GetContentsAsync();
        public Task<bool> DeleteContent(int contentId);
        public void CreateContent(Content content);
        public void UpdateContent(Content content);
        public Task<bool> UpdatePosition(int contentId, int position);
        public Task<bool> SaveChange();
    }
}
