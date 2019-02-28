using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Enviroself.Infrastructure.Jwt.Claims
{
    public class ClaimRequirementFilter : IAsyncActionFilter
    {
        readonly Claim _claim;

        public ClaimRequirementFilter(Claim claim)
        {
            _claim = claim;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool isAuthorized = context.HttpContext.User.Claims.Any(c => c.Type.Equals(_claim.Type) && c.Value.Equals(_claim.Value));
            if (!isAuthorized)
            {
                context.Result = new UnauthorizedResult();
            }
            else
            {
                await next();
            }
        }
    }
}
