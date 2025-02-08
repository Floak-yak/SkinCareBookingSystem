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
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;

        public PostRepository(AppDbContext context) => _context = context;
        public void CreatePost(Post post)
        {
            _context.Posts.Add(post);
        }

        public async Task<bool> DeletePost(int postId)
        {
            Post post = await GetPostByIdAsync(postId);
            if (post is null)
                return false;
            _context.Posts.Remove(post);
            return _context.SaveChanges() > 0;
        }

        public async Task<List<Post>> GetAllPostsAsync() =>
            await _context.Posts
            .Include(p => p.Contents)
            .Include(p => p.User)
            .Include(p => p.Category)
            .ToListAsync();

        public async Task<Post> GetPostByIdAsync(int postId) =>
            await _context.Posts
            .Include(p => p.Contents)
            .Include(p => p.User)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == postId);

        public async Task<Post> GetPostByTitle(string title) =>
            await _context.Posts.FirstOrDefaultAsync(p => p.Title.Equals(title));

        public async Task<bool> IsTitleExist(string title)
        {
            if (await GetPostByTitle(title) is null)
                return true;
            return false;
        }

        public async Task<bool> SaveChange()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void UpdatePost(Post post)
        {
            _context.Update(post);
        }
    }
}
