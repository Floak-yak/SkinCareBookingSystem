using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Service.Service
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<bool> ChangePostStatus(int postId, PostStatus postStatus)
        {
            Post post = await _postRepository.GetPostByIdAsync(postId);
            if (post is null)
                return false;
            post.PostStatus = postStatus;
            _postRepository.UpdatePost(post);
            return await _postRepository.SaveChange();
        }

        public async Task<bool> CreatePost(int userId, string title, List<Content> contents, int categoryId, DateTime datePost, string imgageLink)
        {
            if (string.IsNullOrEmpty(title) || contents is null ||
                string.IsNullOrEmpty(imgageLink) || datePost == DateTime.MinValue) 
                return false;

            if (await _postRepository.IsTitleExist(title)) 
                return false;

            //Check category

            User user = new(); //GetUserById
            Category category = new Category(); //GetCategoryByCategoryId

            if (user is null || category is null)
                return false;

            //Uploadlink to Cloudinary

            Post post = new()
            {
                UserId = userId,
                Title = title,
                Contents = contents,
                CategoryId = categoryId,
                DatePost = datePost,
                ImageLink = imgageLink,
                IsApproved = false,
                Category = category,
                User = user
            };

            _postRepository.CreatePost(post);
            return await _postRepository.SaveChange();
        }

        public async Task<bool> DeletePost(int postId)
        {
            return await _postRepository.DeletePost(postId);
        }

        public Task<List<Post>> GetAllPostsAsync()
        {
            return _postRepository.GetAllPostsAsync();
        }

        public Task<Post> GetPostByIdAsync(int postId)
        {
            return _postRepository.GetPostByIdAsync(postId);    
        }

        public async Task<bool> UpdatePost(string title, int categoryId, string imgageLink)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(imgageLink))
                return false;

            Post post = await _postRepository.GetPostByTitle(title);

            if (post is null)
                return false;

            post.Title = title;
            if (await _postRepository.IsTitleExist(title))
                return false;

            post.CategoryId = categoryId; // Check is category exist
            
            post.ImageLink = imgageLink; 

            _postRepository.UpdatePost(post);
            return await _postRepository.SaveChange();
        }
    }
}
