using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enviroself.Context;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Enviroself.Services.Common
{
    public class CommonService : ICommonService
    {

        #region Fields
        private readonly DbApplicationContext _context;
        #endregion

        #region Ctor
        public CommonService(DbApplicationContext context)
        {
            this._context = context;
        }
        #endregion

        #region Methods
        public virtual async Task<IList<SelectListItem>> GetGenders()
        {
            var result = new List<SelectListItem>()
            {
                new SelectListItem() { Value = "Male", Text = "Male"},
                new SelectListItem() { Value = "Female", Text = "Female"},
            };

            return result;
        }
        #endregion

    }
}
