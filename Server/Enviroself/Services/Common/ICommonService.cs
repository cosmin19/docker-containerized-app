using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enviroself.Services.Common
{
    public interface ICommonService
    {
        Task<IList<SelectListItem>> GetGenders();


    }
}
