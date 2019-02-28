namespace Enviroself.Infrastructure.Jwt.Entities
{
    public static class JwtStaticConstants
    {
        public static class Strings
        {
            public static class JwtClaimIdentifiers
            {
                public const string Rol = "rol", Id = "id", Roles = "roles", Permission = "permission", EmailConfirmed = "emailConfirmed";
            }

            public static class JwtClaims
            {
                public const string ApiAccess = "api_access";
                public const string ApiAccessExtended = "api_access_extended";
            }
        }

    }
}
