using System.Security.Claims;

namespace Jumpeno.Server.Utils;

public static class ClaimsPrinciplesExtenshions
{
    public static string GetSub(this ClaimsPrincipal principal) => principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw Exceptions.NotAuthenticated;

    public static string GetRole(this ClaimsPrincipal principal) => principal.FindFirstValue(ClaimTypes.Role) ?? throw Exceptions.NotAuthenticated;

}
