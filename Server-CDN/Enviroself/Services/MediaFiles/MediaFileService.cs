using Enviroself.Areas.Media.Features.Entity;
using Enviroself.Context;
using Enviroself.Infrastructure.PagingData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enviroself.Services.MediaFiles
{
    public class MediaFileService : IMediaFileService
    {
        #region Fields
        private readonly DbApplicationContext _context;
        #endregion

        #region Ctor
        public MediaFileService(DbApplicationContext context)
        {
            this._context = context;
        }
        #endregion

        #region Methods
        public virtual async Task Create(MediaFile entity)
        {
            await _context.MediaFiles.AddAsync(entity);

            await _context.SaveChangesAsync();
        }

        public virtual async Task Delete(MediaFile entity)
        {
            _context.MediaFiles.Remove(entity);

            await _context.SaveChangesAsync();
        }

        public virtual async Task<IList<MediaFile>> GetAllMediaFiles()
        {
            return await _context.MediaFiles.OrderByDescending(d => d.CreatedOnUtc).ToListAsync();
        }

        public virtual async Task<MediaFile> GetMediaFileById(int id)
        {
            return await _context.MediaFiles.Where(d => d.Id == id).FirstOrDefaultAsync();
        }

        public virtual async Task<int> GetTotalFilesForUser(int userId)
        {
            return await _context.MediaFiles.Where(d => d.UserId == userId).CountAsync();
        }

        public virtual async Task<decimal> GetTotalSizeOfFilesForUser(int userId)
        {
            return await _context.MediaFiles.Where(d => d.UserId == userId).SumAsync(c => c.Size);
        }

        public PagedList<MediaFile> GetAllMediaFilesPagedList(PagingParams pagingParams, int userId)
        {
            IQueryable<MediaFile> entities = _context.MediaFiles;

            entities = entities.Where(c => c.UserId == userId).OrderByDescending(x => x.CreatedOnUtc);

            return new PagedList<MediaFile>(entities, pagingParams.PageNumber, pagingParams.PageSize);
        }

        #endregion

    }
}
