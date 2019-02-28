using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enviroself.Areas.User.Features.Account.Entities
{
    public class ApplicationUserLogin
    {
        public int Id { get; set; }
        public string UserAgent { get; set; }
        public string AcceptLanguage { get; set; }
        public string RemoteIpAddress { get; set; }
        public string LocalIpAddress { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
