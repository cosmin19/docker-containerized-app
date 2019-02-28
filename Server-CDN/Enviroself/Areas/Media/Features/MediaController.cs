using AutoMapper;
using Enviroself.Areas.Media.Features.Dto;
using Enviroself.Areas.Media.Features.Entity;
using Enviroself.Infrastructure.Constants;
using Enviroself.Infrastructure.PagingData;
using Enviroself.Infrastructure.RequestResponse;
using Enviroself.Infrastructure.Utilities;
using Enviroself.Services.Auth;
using Enviroself.Services.MediaFiles;
using Enviroself.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Enviroself.Areas.Media.Features
{
    [Route("api/[controller]/[action]")]
    [Authorize]
    [Authorize(Roles = "USER, ADMIN")]
    public class MediaController : Controller
    {
        #region Fields
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;
        private readonly IMediaFileService _mediaFileService;
        private readonly IMapper _mapper;
        #endregion

        #region Ctor
        public MediaController(
            IUserService userService,
            IIdentityService identityService,
            IMapper mapper,
            IMediaFileService mediaFileService
        )
        {
            this._userService = userService;
            this._identityService = identityService;
            this._mapper = mapper;
            this._mediaFileService = mediaFileService;
        }
        #endregion

        #region Methods

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            // Check if valid user
            var currentUser = await _identityService.GetCurrentPersonIdentityAsync();

            if (currentUser == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Forbidden" });

            if (file == null || file.Length == 0)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "No file selected" });

            // Create a storage folder for user, if not exist
            string folderServerPath = String.Format(FilePathConstants.PUBLIC_USERS_FILES, currentUser.Id);
            if (!Directory.Exists(folderServerPath))
            {
                Directory.CreateDirectory(folderServerPath);
            }
            string fileName = file.FileName;
            string fileServerPath = folderServerPath + file.FileName;
            /* Check if file exists. If so, add a number at the end */
            if(System.IO.File.Exists(fileServerPath))
            {
                for(int i = 1; i < int.MaxValue; i++)
                {
                    var tempFileName = Path.GetFileNameWithoutExtension(file.FileName) + "(" + i + ")" + Path.GetExtension(file.FileName);
                    var tempServerPath = folderServerPath + tempFileName;
                    if (!System.IO.File.Exists(tempServerPath))
                    {
                        fileName = tempFileName;
                        fileServerPath = tempServerPath;
                        break;
                    }
                }
            }

            using (FileStream fileStream = new FileStream(fileServerPath, FileMode.Create, FileAccess.Write))
            {
                // Copy file content
                file.CopyTo(fileStream);

                // Create db entry
                MediaFile entity = new MediaFile()
                {
                    User = currentUser,
                    Title = fileName,
                    Location = fileServerPath,
                    ContentType = file.ContentType,
                    Size = file.Length,
                    CreatedOnUtc = DateTime.UtcNow
                };

                // CLose file stream
                fileStream.Close();

                // Save data to db
                await _mediaFileService.Create(entity);
            }

            // Return success
            return new OkObjectResult(new RequestMessageResponse() { Success = true, Message = "Success" });
        }

        [HttpPost]
        public async Task<IActionResult> GetFiltered([FromBody]MediaFileFilterDto filter)
        {
            // Check if valid user
            var currentUser = await _identityService.GetCurrentPersonIdentityAsync();

            if (currentUser == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Forbidden" });

            // Create pager
            PagingParams pager = new PagingParams();
            pager.PageNumber = filter.PageNumber;
            pager.PageSize = filter.PageSize;

            // Get filtered list from dB
            var fileList = _mediaFileService.GetAllMediaFilesPagedList(pager, currentUser.Id);

            // Map entitites to dto
            IList<MediaFileSmallDto> mediaFileSmallDtos = new List<MediaFileSmallDto>();
            foreach (var file in fileList.List)
            {
                MediaFileSmallDto smallModel = new MediaFileSmallDto()
                {
                    Id = file.Id,
                    Title = file.Title,
                    CreatedOnUtc = file.CreatedOnUtc.ToShortTimeString(),
                    Size = file.Size.GetHumanReadableFileSize()
                };

                mediaFileSmallDtos.Add(smallModel);
            }

            // Initialize result
            PagedListDto<MediaFileSmallDto> returnDto = new PagedListDto<MediaFileSmallDto>();
            returnDto.PagingHeader = fileList.GetHeader();
            returnDto.List = mediaFileSmallDtos;

            // Return result
            return new OkObjectResult(returnDto);
        }


        [HttpGet]
        public async Task<IActionResult> DownloadFile(int fileId)
        {
            // Check if valid user
            var currentUser = await _identityService.GetCurrentPersonIdentityAsync();

            if (currentUser == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Forbidden" });

            // Get file
            var file = await _mediaFileService.GetMediaFileById(fileId);
            if(file == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "File do not exist" });

            // Return file
            try
            {
                var memory = new MemoryStream();
                using (var stream = new FileStream(file.Location, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }

                memory.Position = 0;
                return File(memory, file.ContentType, file.Title);
            }
            catch (Exception)
            {
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Error" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            // Check if valid user
            var currentUser = await _identityService.GetCurrentPersonIdentityAsync();

            if (currentUser == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "Forbidden" });

            // Get file from db
            var file = await _mediaFileService.GetMediaFileById(fileId);
            if (file == null)
                return BadRequest(new RequestMessageResponse() { Success = false, Message = "File do not exist" });

            // If exists, remove from server and Db and return success
            if (System.IO.File.Exists(file.Location))
            {
                try
                {
                    System.IO.File.Delete(file.Location);

                    await this._mediaFileService.Delete(file);
                }
                catch(Exception)
                {
                    return BadRequest(new RequestMessageResponse() { Success = false, Message = "Oops, error. Please try again." });
                }
            }
            // If don't exist, remove from Db and return success
            else
            {
                await this._mediaFileService.Delete(file);
            }

            return new OkObjectResult(new RequestMessageResponse() { Success = true, Message = "Success" });
        }
        #endregion
    }
}
