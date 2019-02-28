using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enviroself.Areas.User.Features.User.Dto
{
    public class UserSmallDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string PictureUrl { get; set; }
        public string Gender { get; set; }

        public int TotalFiles { get; set; }
        public string TotalFilesSize { get; set; }

        public IList<SelectListItem> GenderList { get; set; }
    }
}
