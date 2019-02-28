using System;

namespace Enviroself.Areas.User.Features.Account.Dto
{
    public class RecapthcaResonseDto
    {
        public bool Success { get; set; }
        public DateTime Challenge_ts { get; set; }
        public string Hostname { get; set; }
    }
}
