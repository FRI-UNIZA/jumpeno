namespace Jumpeno.Server.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

public static class JWT {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly string AccessSecret = ServerSettings.Auth.JWT.AccessSecret;
    public static readonly string RefreshSecret = ServerSettings.Auth.JWT.RefreshSecret;
    public static readonly string DataSecret = ServerSettings.Auth.JWT.DataSecret;
    // Issuer & audience:
    public static readonly string Issuer = AppSettings.Name;
    public static readonly string AudienceUser = $"{Issuer} {nameof(User)}";
    public static readonly string AudienceAdmin = $"{Issuer} {nameof(Admin)}";
    // Expiration:
    public static readonly int ExpirationAccess = From.MinToMS(ServerSettings.Expiration.AccessToken.Minutes); // ms
    public static readonly int ExpirationRefresh = From.HourToMS(ServerSettings.Expiration.RefreshToken.Hours); // ms
    public static readonly int ExpirationActivation = From.HourToMS(ServerSettings.Expiration.ActivationToken.Hours); // ms
    public static readonly int ExpirationPasswordReset = From.MinToMS(ServerSettings.Expiration.PasswordResetToken.Minutes); // ms

    // Generators -------------------------------------------------------------------------------------------------------------------------
    private static string Generate(
        TokenType type, string secret, int expiration,
        string sub, Role role, string aud,
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
            issuer: Issuer,
            audience: aud,
            claims: claims,
            expires: DateTime.UtcNow.AddMilliseconds(expiration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateAdminAccess(string email) => Generate(
        TokenType.Access, AccessSecret, ExpirationAccess,
        email, Role.Admin, AudienceAdmin
    );
    public static string GenerateAdminRefresh(string email) => Generate(
        TokenType.Refresh, RefreshSecret, ExpirationRefresh,
        email, Role.Admin, AudienceAdmin,
        $"{nameof(Guid)}:{Guid.NewGuid()}"
    );

    public static string GenerateUserAccess(Guid id) => Generate(
        TokenType.Access, AccessSecret, ExpirationAccess,
        id.ToString(), Role.User, AudienceUser
    );
    public static string GenerateUserRefresh(Guid id) => Generate(
        TokenType.Refresh, RefreshSecret, ExpirationRefresh,
        id.ToString(), Role.User, AudienceUser,
        $"{nameof(Guid)}:{Guid.NewGuid()}"
    );

    public static string GenerateActivation(Guid id) => Generate(
        TokenType.Activation, DataSecret, ExpirationActivation,
        id.ToString(), Role.User, AudienceUser
    );

    public static string GeneratePasswordReset(string email, string password) => Generate(
        TokenType.PasswordReset, DataSecret, ExpirationPasswordReset,
        email, Role.User, AudienceUser,
        password
    );

    // Validation -------------------------------------------------------------------------------------------------------------------------
    private static bool Validate(TokenType type, string secret, string token) {
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
    private static string Assert(TokenType type, string secret, string token) {
        return Validate(type, secret, token) ? token : throw Exceptions.NotAuthenticated;
    }

    public static bool ValidateAccess(string token) => Validate(TokenType.Access, AccessSecret, token);
    public static string AssertAccess(string token) => Assert(TokenType.Access, AccessSecret, token);
    
    public static bool ValidateRefresh(string token) => Validate(TokenType.Refresh, RefreshSecret, token);
    public static string AssertRefresh(string token) => Assert(TokenType.Refresh, RefreshSecret, token);
    
    public static bool ValidateActivation(string token) => Validate(TokenType.Activation, DataSecret, token);
    public static string AssertActivation(string token) => Assert(TokenType.Activation, DataSecret, token);
    
    public static bool ValidatePasswordReset(string token) => Validate(TokenType.PasswordReset, DataSecret, token);
    public static string AssertPasswordReset(string token) => Assert(TokenType.PasswordReset, DataSecret, token);

    // Refresh ----------------------------------------------------------------------------------------------------------------------------
    public static void SetRefreshTokenCookie(string token) {
        var cookieStorage = AppEnvironment.GetService<CookieStorage>();
        var expires = DateTimeOffset.UtcNow.AddMilliseconds(ExpirationRefresh);
        var cookie = new Client.Models.Cookie(
            Cookies.Mandatory.AppRefershToken, token,
            expires: expires,
            path: API.Base.AuthRefresh, httpOnly: true, secure: true
        );
        cookieStorage.Set(cookie);
        cookie = new Client.Models.Cookie(
            Cookies.Mandatory.AppRefershToken, token,
            expires: expires,
            path: API.Base.AuthInvalidate, httpOnly: true, secure: true
        );
        cookieStorage.Set(cookie);
        cookie = new Client.Models.Cookie(
            Cookies.Mandatory.AppRefershToken, token,
            expires: expires,
            path: API.Base.AuthDelete, httpOnly: true, secure: true
        );
        cookieStorage.Set(cookie);
    }

    public static void DeleteRefreshTokenCookie() {
        var cookieStorage = AppEnvironment.GetService<CookieStorage>();
        cookieStorage.Delete(Cookies.Mandatory.AppRefershToken, path: API.Base.AuthRefresh);
        cookieStorage.Delete(Cookies.Mandatory.AppRefershToken, path: API.Base.AuthInvalidate);
        cookieStorage.Delete(Cookies.Mandatory.AppRefershToken, path: API.Base.AuthDelete);
    }

    // Authorization ----------------------------------------------------------------------------------------------------------------------
    public static void Authorize(string token, Role[] roles) {
        // Validate token:
        if (!ValidateAccess(token)) throw Exceptions.NotAuthenticated;
        // Store token:
        Token.StoreAccess(token);
        // Check roles:
        bool allowed = false;
        foreach (var role in roles) {
            if (role == Token.Access.role) {
                allowed = true;
                break;
            }
        }
        if (!allowed) throw Exceptions.NotAuthorized;
    }
    
    public static void Authorize(HttpContext ctx) {
        // Get endpoint metadata:
        var endpoint = ctx.GetEndpoint();
        if (endpoint == null) return;
        
        // Get controller and action metadata:
        var controllerActionDescriptor = endpoint.Metadata
            .OfType<Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor>()
            .FirstOrDefault();
        if (controllerActionDescriptor == null) return;
        
        // Get the method info:
        var methodInfo = controllerActionDescriptor.MethodInfo;
        if (methodInfo == null) return;

        // Get the custom RoleAttribute:
        var roleAttribute = methodInfo.GetCustomAttribute<RoleAttribute>();
        if (roleAttribute == null) return;
            
        // Get token:
        var authHeader = ctx.Request.Headers.Authorization.FirstOrDefault();
        if (!(!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith($"{AuthTypes.Bearer} "))) throw Exceptions.NotAuthenticated;
        string token = authHeader.Substring($"{AuthTypes.Bearer} ".Length).Trim();

        // Authorize:
        Authorize(token, roleAttribute.Allowed);
    }
}
