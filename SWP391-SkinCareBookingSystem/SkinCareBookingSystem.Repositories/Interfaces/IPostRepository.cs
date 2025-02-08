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
        public Task<Post> GetPostByIdAsync();
        public void DeletePost(int postId);
        public void UpdatePost(Post post);
        public void CreatePost(Post post);
    }
}
