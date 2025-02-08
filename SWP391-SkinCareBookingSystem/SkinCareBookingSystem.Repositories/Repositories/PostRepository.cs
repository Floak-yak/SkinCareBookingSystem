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
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context) => _context = context;
        public void CreatePost(Post post)
        {
            throw new NotImplementedException();
        }

        public void DeletePost(int postId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Post>> GetAllPostsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Post> GetPostByIdAsync()
        {
            throw new NotImplementedException();
        }

        public void UpdatePost(Post post)
        {
            throw new NotImplementedException();
        }
    }
}
