using Enviroself.Infrastructure.PagingData;

namespace Enviroself.Areas.Admin.Features.User.Dto
{
    public class UserFilterAdminDto : BaseFilter
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

    }
}
