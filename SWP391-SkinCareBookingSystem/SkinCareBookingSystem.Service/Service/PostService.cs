using SkinCareBookingSystem.BusinessObject.Dto;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Repositories.Repositories;
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
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPostRepository _postRepository;
        private readonly IImageService _imageService;

        public PostService(IPostRepository postRepository, IImageService imageService, ICategoryRepository categoryRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _postRepository = postRepository;
            _imageService = imageService;
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

        public async Task<bool> CreatePost(int userId, string title, List<CreatePostContentRequest> contents, int categoryId, string imageLink)
        {
            if (string.IsNullOrEmpty(title) || contents is null ||
                string.IsNullOrEmpty(imageLink)) 
                return false;

            if (await _postRepository.IsTitleExist(title)) 
                return false;

            User user = await _userRepository.GetUserById(userId);
            if (user is null)
                return false;

            Category category = await _categoryRepository.GetCategoryById(userId);
            if (category is null)
                return false;

            if (!await _imageService.StoreImage(imageLink))
                return false;

            Image image = await _imageService.GetImageByDescription(Path.GetFileName(imageLink));

            List<Content> listContent = new();
            foreach (var content in contents)
            {
                listContent.Add(new Content() { });
            }

            Post post = new()
            {
                UserId = userId,
                Title = title,
                Contents = listContent,
                CategoryId = categoryId,
                DatePost = DateTime.UtcNow,
                Image = image,
                PostStatus = 0,
                Category = category,
                User = user
            };

            _postRepository.CreatePost(post);
            return await _postRepository.SaveChange();
        }

        public async Task<bool> CreatePostWithoutContent(int userId, string title, int categoryId, string imageLink)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(imageLink))
                return false;

            if (await _postRepository.IsTitleExist(title))
                return false;

            User user = await _userRepository.GetUserById(userId);
            if (user is null)
                return false;

            Category category = await _categoryRepository.GetCategoryById(userId);
            if (category is null)
                return false;

            if (!await _imageService.StoreImage(imageLink))
                return false;

            Image image = await _imageService.GetImageByDescription(Path.GetFileName(imageLink));

            Post post = new()
            {
                UserId = userId,
                Title = title,
                CategoryId = categoryId,
                DatePost = DateTime.UtcNow,
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

        public async Task<bool> UpdatePost(int postId, string title, int categoryId, string imageLink)
        {
            Post post = await _postRepository.GetPostByIdAsync(postId);

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
                if (!await _imageService.StoreImage(imageLink))
                    return false;
                post.Image = await _imageService
                    .GetImageByDescription(Path.GetFileName(imageLink));
            }
            
            _postRepository.UpdatePost(post);
            return await _postRepository.SaveChange();
        }
    }
} 
