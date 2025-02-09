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
    public class PostContentSerivce : IPostContentSerivce
    {
        private readonly IPostContentRepository _postContentRepository;
        private readonly IPostRepository _postRepository;

        public PostContentSerivce(IPostContentRepository postContentRepository, IPostRepository postRepository)
        {
            _postContentRepository = postContentRepository;
            _postRepository = postRepository;
        }
        public async Task<bool> CreateContent(string contentOfPost, ContentType contentType, int position, string imageLink, int postId)
        {
            if (string.IsNullOrEmpty(contentOfPost) || string.IsNullOrEmpty(imageLink))
                return false;

            //Check imageLink work or not
            Image image = new();

            Post post = await _postRepository.GetPostByIdAsync(postId);
            if (post is null)
                return false;

            Content content = new()
            {
                ContentOfPost = contentOfPost,
                ContentType = contentType,
                Position = position,
                Image = image,
                PostId = postId,
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

        public async Task<bool> UpdateContent(int contentId, string contentOfPost, ContentType contentType, int position, string imageLink)
        {
            Content content = await _postContentRepository.GetContentByIdAsync(contentId);

            if (content is null)
                return false;

            if (string.IsNullOrEmpty(contentOfPost))
                content.ContentType = contentType;

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
