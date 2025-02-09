using Microsoft.EntityFrameworkCore;
using SkinCareBookingSystem.BusinessObject.Entity;
using SkinCareBookingSystem.Repositories.Data;
using SkinCareBookingSystem.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SkinCareBookingSystem.Repositories.Repositories
{
    public class PostContentRepository : IPostContentRepository
    {
        private readonly AppDbContext _context;

        public PostContentRepository(AppDbContext context) => _context = context; 
        public void CreateContent(Content content)
        {
            _context.Contents.Add(content);
        }

        public async Task<bool> SaveChange()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteContent(int contentId)
        {
            Content content = await GetContentByIdAsync(contentId);
            _context.Contents.Remove(content);
            return await SaveChange();
        }

        public async Task<Content> GetContentByIdAsync(int contentId) =>
             await _context.Contents
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == contentId);

        public async Task<List<Content>> GetContentsAsync() =>
            await _context.Contents
                .Include(c => c.Post)
                .ToListAsync();

        public void UpdateContent(Content content)
        {
            _context.Contents.Update(content);
        }


        /// <summary>
        /// Insert after the object
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePosition(int contentId, int position)
        {
            Content content = await GetContentByIdAsync(contentId);
            
            if (content is null)
                return false;
            
            int contentOldPosition = content.Position;
            List<Content> listContent = await _context.Contents.Where(c => c.PostId == content.PostId).OrderBy(c => c.Position).ToListAsync();

            if (position > content.Position)
                foreach (Content item in listContent)
                {
                    if (item.Position < contentOldPosition && item.Id != contentId)
                        continue;
                    else
                        item.Position -= 1;
                    if (contentId == item.PostId)
                        content.Position = position;
                    if (item.Position > position)
                        break;
                }
            else
                foreach (Content item in listContent)
                {
                    if (item.Position < contentOldPosition && item.Id != contentId)
                        continue;
                    else
                        item.Position += 1;
                    if (contentId == item.PostId)
                        content.Position = position;
                    if (item.Position > contentOldPosition)
                        break;
                }

            _context.Contents.UpdateRange(listContent);
            return await SaveChange();
        }
    }
}
