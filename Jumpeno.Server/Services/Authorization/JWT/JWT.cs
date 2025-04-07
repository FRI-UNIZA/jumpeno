namespace Jumpeno.Server.Utils;

using System.Reflection;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public static class JWT {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly string ACCESS_SECRET = ServerSettings.Auth.JWT.AccessSecret;
    public static readonly string REFRESH_SECRET = ServerSettings.Auth.JWT.RefreshSecret;
    public static readonly string DATA_SECRET = ServerSettings.Auth.JWT.DataSecret;
    // Issuer & audience:
    public static readonly string ISSUER = AppSettings.Name;
    public static readonly string AUDIENCE_USERS = $"{ISSUER} users";
    public static readonly string AUDIENCE_ADMINS = $"{ISSUER} admins";
    // Expiration:
    public const int EXPIRATION_ACCESS_MS = 3_600_000; // 1 hour
    public const int EXPIRATION_REFRESH_MS = 604_800_000; // 7 days
    public const int EXPIRATION_ACTIVATION_MS = 259_200_000; // 3 days
    public const int EXPIRATION_PASSWORD_RESET_MS = 300_000; // 5 minutes

    // Generators -------------------------------------------------------------------------------------------------------------------------
    private static string Generate(TOKEN_TYPE type, string secret, int expiration, string sub, ROLE role, string aud) {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256
        );

        var claims = new[] {
            new Claim(nameof(Token.Access.type), type.ToString()),
            new Claim(nameof(Token.Access.sub), sub),
            new Claim(nameof(Token.Access.role), role.ToString()),
            new Claim(nameof(Token.Access.iat), DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: ISSUER,
            audience: aud,
            claims: claims,
            expires: DateTime.UtcNow.AddMilliseconds(expiration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateAdminAccess(string email) => Generate(
        TOKEN_TYPE.ACCESS, ACCESS_SECRET, EXPIRATION_ACCESS_MS,
        email, ROLE.ADMIN, AUDIENCE_ADMINS
    );
    public static string GenerateAdminRefresh(string email) => Generate(
        TOKEN_TYPE.REFRESH, REFRESH_SECRET, EXPIRATION_REFRESH_MS,
        email, ROLE.ADMIN, AUDIENCE_ADMINS
    );

    public static string GenerateUserAccess(Guid id) => Generate(
        TOKEN_TYPE.ACCESS, ACCESS_SECRET, EXPIRATION_ACCESS_MS,
        id.ToString(), ROLE.USER, AUDIENCE_USERS
    );
    public static string GenerateUserRefresh(Guid id) => Generate(
        TOKEN_TYPE.REFRESH, REFRESH_SECRET, EXPIRATION_REFRESH_MS,
        id.ToString(), ROLE.USER, AUDIENCE_USERS
    );

    public static string GenerateActivation(Guid id) => Generate(
        TOKEN_TYPE.ACTIVATION, DATA_SECRET, EXPIRATION_ACTIVATION_MS,
        id.ToString(), ROLE.USER, AUDIENCE_USERS
    );

    // Validation -------------------------------------------------------------------------------------------------------------------------
    private static bool Validate(string secret, string token) {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secret);

        var validationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        try {
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return true;
        } catch {
            return false;
        }
    }
    private static string Check(string secret, string token) => Validate(secret, token) ? token : throw Exceptions.NotAuthenticated;

    public static bool ValidateAccess(string token) => Validate(ACCESS_SECRET, token);
    public static string CheckAccess(string token) => Check(ACCESS_SECRET, token);
    
    public static bool ValidateRefresh(string token) => Validate(REFRESH_SECRET, token);
    public static string CheckRefresh(string token) => Check(REFRESH_SECRET, token);
    
    public static bool ValidateActivation(string token) => Validate(DATA_SECRET, token);
    public static string CheckActivation(string token) => Check(DATA_SECRET, token);

    // Authorization ----------------------------------------------------------------------------------------------------------------------
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
            if (!ValidateAccess(token)) throw Exceptions.NotAuthenticated;
            var data = Token.Decode(token) ?? throw Exceptions.NotAuthenticated;

            bool allowed = false;
            foreach (var role in roleAttribute.Allowed) {
                if (role == data.role) {
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
