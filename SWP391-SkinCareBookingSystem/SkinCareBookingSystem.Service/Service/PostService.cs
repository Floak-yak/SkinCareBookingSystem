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

        public async Task<bool> CreatePost(int userId, string title, List<Content> contents, int categoryId, DateTime datePost, string imageLink)
        {
            if (string.IsNullOrEmpty(title) || contents is null ||
                string.IsNullOrEmpty(imageLink) || datePost == DateTime.MinValue) 
                return false;

            if (await _postRepository.IsTitleExist(title)) 
                return false;

            //Check category

            User user = new(); //GetUserById
            Category category = new Category(); //GetCategoryByCategoryId

            if (user is null || category is null)
                return false;

            //Uploadlink to Cloudinary
            Image image = new();

            Post post = new()
            {
                UserId = userId,
                Title = title,
                Contents = contents,
                CategoryId = categoryId,
                DatePost = datePost,
                Image = image,
                PostStatus = 0,
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

        public async Task<bool> UpdatePost(string title, int categoryId, string imageLink)
        {
            Post post = await _postRepository.GetPostByTitle(title);

            if (post is null)
                return false;

            if (!string.IsNullOrEmpty(title))
            {
                if (await _postRepository.IsTitleExist(title))
                    return false;
                post.Title = title;
            }

            post.CategoryId = categoryId; // Check is category exist

            if (!string.IsNullOrEmpty(imageLink))
            {
                //Check link working or not
                Image image = new();
                post.Image = image;
            }
            
            _postRepository.UpdatePost(post);
            return await _postRepository.SaveChange();
        }
    }
}
