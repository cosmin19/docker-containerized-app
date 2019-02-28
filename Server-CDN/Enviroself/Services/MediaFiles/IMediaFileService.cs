using Enviroself.Areas.Media.Features.Entity;
using Enviroself.Infrastructure.PagingData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enviroself.Services.MediaFiles
{
    public interface IMediaFileService
    {
        Task Create(MediaFile entity);
        Task Delete(MediaFile entity);
        Task<MediaFile> GetMediaFileById(int id);
        Task<IList<MediaFile>> GetAllMediaFiles();
        Task<int> GetTotalFilesForUser(int userId);
        Task<decimal> GetTotalSizeOfFilesForUser(int userId);
        PagedList<MediaFile> GetAllMediaFilesPagedList(PagingParams pagingParams, int userId);
    }
}
