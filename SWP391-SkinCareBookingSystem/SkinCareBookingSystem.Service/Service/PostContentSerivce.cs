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
    public class PostContentSerivce : IPostContentSerivce
    {
        private readonly IPostContentRepository _postContentRepository;
        private readonly IPostRepository _postRepository;

        public PostContentSerivce(IPostContentRepository postContentRepository, IPostRepository postRepository)
        {
            _postContentRepository = postContentRepository;
            _postRepository = postRepository;
        }
        public async Task<bool> CreateContent(CreatePostContentRequest request)
        {
            if (string.IsNullOrEmpty(request.ContentOfPost) || string.IsNullOrEmpty(request.imageLink))
                return false;

            //Check imageLink work or not
            Image image = new();

            Post post = await _postRepository.GetPostByIdAsync(request.PostId);
            if (post is null)
                return false;

            Content content = new()
            {
                ContentOfPost = request.ContentOfPost,
                Position = request.Position,
                Image = image,
                PostId = request.PostId,
                Post = post,
            };

            _postContentRepository.CreateContent(content);

            return await _postContentRepository.SaveChange();
        }

        public async Task<bool> DeleteContent(int contentId) =>
            await _postContentRepository.DeleteContent(contentId);

        public async Task<Content> GetContentByIdAsync(int contentId) =>
            await _postContentRepository.GetContentByIdAsync(contentId);

        public async Task<List<Content>> GetContentsAsync() =>
            await _postContentRepository.GetContentsAsync();

        public async Task<bool> UpdateContent(int contentId, string contentOfPost, int position, string imageLink)
        {
            Content content = await _postContentRepository.GetContentByIdAsync(contentId);

            if (content is null)
                return false;

            if (string.IsNullOrEmpty(contentOfPost))

            if (string.IsNullOrEmpty(imageLink))
            {
                //Check the link
                Image image = new();
                content.Image = image;
            }

            if (!await _postContentRepository.UpdatePosition(contentId, position))
                return false;
            content.Position = position;

            _postContentRepository.UpdateContent(content);
            return await _postContentRepository.SaveChange();
        }
    }
}
