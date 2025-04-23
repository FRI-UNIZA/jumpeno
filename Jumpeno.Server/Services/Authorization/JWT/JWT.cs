namespace Jumpeno.Server.Services;

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
    public static readonly string AUDIENCE_USER = $"{ISSUER} {nameof(User)}";
    public static readonly string AUDIENCE_ADMIN = $"{ISSUER} {nameof(Admin)}";
    // Expiration:
    public static readonly int EXPIRATION_ACCESS = From.MinToMS(ServerSettings.Expiration.AccessToken.Minutes); // ms
    public static readonly int EXPIRATION_REFRESH = From.HourToMS(ServerSettings.Expiration.RefreshToken.Hours); // ms
    public static readonly int EXPIRATION_ACTIVATION = From.HourToMS(ServerSettings.Expiration.ActivationToken.Hours); // ms
    public static readonly int EXPIRATION_PASSWORD_RESET = From.MinToMS(ServerSettings.Expiration.PasswordResetToken.Minutes); // ms

    // Generators -------------------------------------------------------------------------------------------------------------------------
    private static string Generate(
        TOKEN_TYPE type, string secret, int expiration,
        string sub, ROLE role, string aud,
        string data = ""
    ) {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256
        );

        var claims = new[] {
            new Claim(nameof(Token.Data.type), type.ToString()),
            new Claim(nameof(Token.Data.sub), sub),
            new Claim(nameof(Token.Data.role), role.ToString()),
            new Claim(nameof(Token.Data.data), data),
            new Claim(nameof(Token.Data.iat), DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
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
        TOKEN_TYPE.ACCESS, ACCESS_SECRET, EXPIRATION_ACCESS,
        email, ROLE.ADMIN, AUDIENCE_ADMIN
    );
    public static string GenerateAdminRefresh(string email) => Generate(
        TOKEN_TYPE.REFRESH, REFRESH_SECRET, EXPIRATION_REFRESH,
        email, ROLE.ADMIN, AUDIENCE_ADMIN,
        $"{nameof(Guid)}:{Guid.NewGuid()}"
    );

    public static string GenerateUserAccess(Guid id) => Generate(
        TOKEN_TYPE.ACCESS, ACCESS_SECRET, EXPIRATION_ACCESS,
        id.ToString(), ROLE.USER, AUDIENCE_USER
    );
    public static string GenerateUserRefresh(Guid id) => Generate(
        TOKEN_TYPE.REFRESH, REFRESH_SECRET, EXPIRATION_REFRESH,
        id.ToString(), ROLE.USER, AUDIENCE_USER,
        $"{nameof(Guid)}:{Guid.NewGuid()}"
    );

    public static string GenerateActivation(Guid id) => Generate(
        TOKEN_TYPE.ACTIVATION, DATA_SECRET, EXPIRATION_ACTIVATION,
        id.ToString(), ROLE.USER, AUDIENCE_USER
    );

    public static string GeneratePasswordReset(string email, string password) => Generate(
        TOKEN_TYPE.PASSWORD_RESET, DATA_SECRET, EXPIRATION_PASSWORD_RESET,
        email, ROLE.USER, AUDIENCE_USER,
        password
    );

    // Validation -------------------------------------------------------------------------------------------------------------------------
    private static bool Validate(TOKEN_TYPE type, string secret, string token) {
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
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            if (principal == null) return false;
            if (principal.FindFirst(nameof(Token.Data.type))?.Value != type.ToString()) return false;
            return true;
        } catch {
            return false;
        }
    }
    private static string Check(TOKEN_TYPE type, string secret, string token) {
        return Validate(type, secret, token) ? token : throw Exceptions.NotAuthenticated;
    }

    public static bool ValidateAccess(string token) => Validate(TOKEN_TYPE.ACCESS, ACCESS_SECRET, token);
    public static string CheckAccess(string token) => Check(TOKEN_TYPE.ACCESS, ACCESS_SECRET, token);
    
    public static bool ValidateRefresh(string token) => Validate(TOKEN_TYPE.REFRESH, REFRESH_SECRET, token);
    public static string CheckRefresh(string token) => Check(TOKEN_TYPE.REFRESH, REFRESH_SECRET, token);
    
    public static bool ValidateActivation(string token) => Validate(TOKEN_TYPE.ACTIVATION, DATA_SECRET, token);
    public static string CheckActivation(string token) => Check(TOKEN_TYPE.ACTIVATION, DATA_SECRET, token);
    
    public static bool ValidatePasswordReset(string token) => Validate(TOKEN_TYPE.PASSWORD_RESET, DATA_SECRET, token);
    public static string CheckPasswordReset(string token) => Check(TOKEN_TYPE.PASSWORD_RESET, DATA_SECRET, token);

    // Refresh ----------------------------------------------------------------------------------------------------------------------------
    public static void SetRefreshTokenCookie(string token) {
        var expires = DateTimeOffset.UtcNow.AddMilliseconds(EXPIRATION_REFRESH);
        var cookie = new Cookie(
            COOKIE_MANDATORY.APP_REFRESH_TOKEN, token,
            expires: expires,
            path: API.BASE.AUTH_REFRESH, httpOnly: true, secure: true
        );
        CookieStorage.Set(cookie);
        cookie = new Cookie(
            COOKIE_MANDATORY.APP_REFRESH_TOKEN, token,
            expires: expires,
            path: API.BASE.AUTH_INVALIDATE, httpOnly: true, secure: true
        );
        CookieStorage.Set(cookie);
        cookie = new Cookie(
            COOKIE_MANDATORY.APP_REFRESH_TOKEN, token,
            expires: expires,
            path: API.BASE.AUTH_DELETE, httpOnly: true, secure: true
        );
        CookieStorage.Set(cookie);
    }

    public static void DeleteRefreshTokenCookie() {
        CookieStorage.Delete(COOKIE_MANDATORY.APP_REFRESH_TOKEN, path: API.BASE.AUTH_REFRESH);
        CookieStorage.Delete(COOKIE_MANDATORY.APP_REFRESH_TOKEN, path: API.BASE.AUTH_INVALIDATE);
        CookieStorage.Delete(COOKIE_MANDATORY.APP_REFRESH_TOKEN, path: API.BASE.AUTH_DELETE);
    }

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
            
            var authHeader = ctx.Request.Headers.Authorization.FirstOrDefault();
            if (!(!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith($"{AUTH.BEARER} "))) throw Exceptions.NotAuthenticated;
            
            string token = authHeader.Substring($"{AUTH.BEARER} ".Length).Trim();
            if (!ValidateAccess(token)) throw Exceptions.NotAuthenticated;

            Token.StoreAccess(token);
            bool allowed = false;
            foreach (var role in roleAttribute.Allowed) {
                if (role == Token.Access.role) {
                    allowed = true;
                    break;
                }
            }
            if (!allowed) throw Exceptions.NotAuthorized;
        }
    }
}
