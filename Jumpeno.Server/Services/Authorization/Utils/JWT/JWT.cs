namespace Jumpeno.Server.Utils;

using System.Reflection;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public static class JWT {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    private static readonly string SECRET = ServerSettings.JWT.AccessSecret;
    private const string ISSUER = "Jumpeno";
    private const string AUDIENCE_USERS = "Jumpeno users";
    private const string AUDIENCE_ADMINS = "Jumpeno admins";

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static string GenerateAdmin(string email, int expiresMS = 60 * 60 * 1000) {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET)),
            SecurityAlgorithms.HmacSha256
        );

        var claims = new[] {
            new Claim(nameof(AccessToken.sub), email),
            new Claim(nameof(AccessToken.role), ROLE.ADMIN.ToString()),
            new Claim(nameof(AccessToken.iat), DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: ISSUER,
            audience: AUDIENCE_ADMINS,
            claims: claims,
            expires: DateTime.UtcNow.AddMilliseconds(expiresMS),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateUser(Guid id, int expiresMS = 60 * 60 * 1000) {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECRET)),
            SecurityAlgorithms.HmacSha256
        );

        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
            new Claim(ClaimTypes.Role, ROLE.USER.ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: ISSUER,
            audience: AUDIENCE_USERS,
            claims: claims,
            expires: DateTime.UtcNow.AddMilliseconds(expiresMS),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static ClaimsPrincipal? Validate(string token) {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(SECRET);

        var validationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false, // Set to true if you want to check issuer
            ValidateAudience = false, // Set to true if you want to check audience
            ValidateLifetime = true, // Ensures token is not expired
            ClockSkew = TimeSpan.Zero // No tolerance for token expiration
        };

        try {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            Console.WriteLine("Token is valid!");
            return principal;
        } catch (Exception ex) {
            Console.WriteLine($"Token validation failed: {ex.Message}");
            return null;
        }
    }

    public static void Authorize(HttpContext ctx) {
        // Get endpoint metadata
        var endpoint = ctx.GetEndpoint();
        if (endpoint == null) return;
        
        // Get controller and action metadata
        var controllerActionDescriptor = endpoint.Metadata
            .OfType<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()
            .FirstOrDefault();
        if (controllerActionDescriptor == null) return;
        
        // Get the method info
        var methodInfo = controllerActionDescriptor.MethodInfo;
        if (methodInfo == null) return;

        // Get the custom RoleAttribute
        var roleAttribute = methodInfo.GetCustomAttribute<RoleAttribute>();
        if (roleAttribute != null) {
            
            var authHeader = ctx.Request.Headers["Authorization"].FirstOrDefault();
            if (!(!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))) {
                throw Exceptions.NotAuthenticated;
            }
            
            string token = authHeader.Substring("Bearer ".Length).Trim();
            var principal = Validate(token);
            if (principal == null) {
                throw Exceptions.NotAuthenticated;
            }
            Console.WriteLine("Token is valid!!!");
            var tokenRole = principal.FindFirst(ClaimTypes.Role)?.Value;
            Console.WriteLine($"tokenRole: {tokenRole}");

            if (tokenRole == null) {
                throw Exceptions.NotAuthorized;
            }

            bool allowed = false;
            foreach (var role in roleAttribute.Allowed) {
                if (role.ToString() == tokenRole.ToString()) {
                    allowed = true;
                    break;
                }
                Console.WriteLine("Allowed role: " + role);
            }
            if (!allowed) {
                throw Exceptions.NotAuthorized;
            }
            Console.WriteLine("FINALLY AUTHORIZED!!!");
        }
    }
}
