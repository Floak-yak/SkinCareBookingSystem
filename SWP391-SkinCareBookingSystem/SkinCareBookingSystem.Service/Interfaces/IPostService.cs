using SkinCareBookingSystem.BusinessObject.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Interfaces
{
    public interface IPostService
    {
        public Task<List<Post>> GetAllPostsAsync();
        public Task<Post> GetPostByIdAsync(int postId);
        public Task<bool> DeletePost(int postId);
        /// <summary>
        /// This method only update post titel, category
        /// </summary>
        /// <param name="title"></param>
        /// <param name="categoryId"></param>
        /// <param name="datePost"></param>
        /// <param name="imgageLink"></param>
        /// <returns></returns>
        public Task<bool> UpdatePost(int postId, string title, int categoryId, string imageLink);
        public Task<bool> ChangePostStatus(int postId, PostStatus postStatus);
        public Task<bool> CreatePost(int userId, string title, List<Content> contents, int categoryId, DateTime datePost, string imageLink);
    }
}
