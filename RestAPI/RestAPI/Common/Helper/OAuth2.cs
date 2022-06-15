using RestAPI.Common.Enums;

namespace RestAPI.Common.Helper
{
    public static class OAuth2
    {
        public static Guid UserGuid(IHttpContextAccessor httpContextAccessor)
        {
            return new Guid(
                httpContextAccessor.HttpContext?.User.Claims.First(c => c.Type == "UserId").Value
                    ?? throw new ArgumentNullException(nameof(httpContextAccessor.HttpContext))
                );
        }
        
        public static EUserType UserRole(IHttpContextAccessor httpContextAccessor)
        {
            return Enum.Parse<EUserType>(
                httpContextAccessor.HttpContext.User.Claims
                    .First(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value
            );
        }
    }
}
