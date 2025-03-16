using HtmlAgilityPack;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Interfaces;
using SkinCareBookingSystem.Repositories.Repositories;
using SkinCareBookingSystem.Service.Dto;
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

        public async Task<bool> CreatePost(CreatePostWithContentsRequest request)
        {
            if (string.IsNullOrEmpty(request.Title))
                throw new ArgumentNullException(nameof(request.Title));

            if (string.IsNullOrEmpty(request.imageLink))
                throw new ArgumentNullException(nameof(request.imageLink));

            if (string.IsNullOrEmpty(request.content))
                throw new ArgumentNullException(nameof(request.content));

            if (await _postRepository.IsTitleExist(request.Title)) 
                return false;

            User user = await _userRepository.GetUserById(request.UserId);
            if (user is null)
                throw new ArgumentNullException(typeof(User).ToString());

            Category category = await _categoryRepository.GetCategoryById(request.UserId);
            if (category is null)
                throw new ArgumentNullException(typeof(Category).ToString());

            if (!await _imageService.StoreImage(request.imageLink))
                return false;

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(request.content);

            Image image = await _imageService.AddImage(request.imageLink);

            Post post = new()
            {
                UserId = request.UserId,
                Title = request.Title,
                Contents = await GetContentsWithImages(htmlDocument),
                CategoryId = request.CategoryId,
                DatePost = DateTime.UtcNow,
                Image = image,
                PostStatus = 0,
                Category = category,
                User = user
            };

            _postRepository.CreatePost(post);
            return await _postRepository.SaveChange();
        }

        public async Task<List<Content>> GetContentsWithImages(HtmlDocument doc)
        {
            var contents = new List<Content>();

            var contentNodes = doc.DocumentNode.SelectSingleNode("//div[@class='content-section']");
            if (contentNodes == null)
                return contents;

            var nodes = contentNodes.ChildNodes;

            int position = 1;
            Image? lastImage = null;

            foreach (var node in nodes)
            {
                if (node.Name == "p")
                {
                    var contentText = node.InnerText.Trim();
                    contents.Add(new Content
                    {
                        Id = position,
                        ContentOfPost = contentText,
                        Position = position++,
                        PostId = 1,
                        Image = lastImage
                    });

                    lastImage = null;
                }
                else if (node.Name == "img")
                {
                    lastImage = await _imageService.AddImage(node.GetAttributeValue("src", null));
                }
            }
            return contents;
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

        public async Task<List<Post>> Search(string seachText) =>
            await _postRepository.Search(seachText);

        public async Task<List<Post>> Search(int categoryId) =>
            await _postRepository.Search(categoryId);

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
