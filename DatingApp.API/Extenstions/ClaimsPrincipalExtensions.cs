using System.Security.Claims;

namespace DatingApp.API.Extenstions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserName(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
