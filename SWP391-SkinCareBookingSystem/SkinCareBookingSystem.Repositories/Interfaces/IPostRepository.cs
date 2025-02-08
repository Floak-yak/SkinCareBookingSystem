using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Interfaces
{
    public interface IPostRepository
    {
        public Task<List<Post>> GetAllPostsAsync();
        public Task<Post> GetPostByIdAsync(int postId);
        public Task<bool> DeletePost(int postId);
        public void UpdatePost(Post post);
        public void CreatePost(Post post);
        public Task<bool> SaveChange();
        public Task<Post> GetPostByTitle(string title);
        public Task<bool> IsTitleExist(string title);
    }
}
