using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enviroself.Areas.Media.Features.Dto
{
    public class MediaFileSmallDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Size { get; set; }
        public string CreatedOnUtc { get; set; }
    }
}
