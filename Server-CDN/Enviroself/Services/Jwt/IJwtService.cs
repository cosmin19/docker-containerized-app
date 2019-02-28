using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Enviroself.Services.Jwt
{
    public interface IJwtService
    {
        ClaimsIdentity GenerateClaimsIdentity(string userName, int id, bool emailConfirmed, IList<string> roles, IList<string> claims);

        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);

        int GetValidForTotalSeconds();
    }
}
