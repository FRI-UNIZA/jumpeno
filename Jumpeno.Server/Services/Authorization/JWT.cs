namespace Jumpeno.Server.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            issuer: ISSUER,
            audience: aud,
            claims: claims,
            expires: DateTime.UtcNow.AddMilliseconds(expiration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateAdminAccess(string email) => Generate(
        TokenType.ACCESS, ACCESS_SECRET, EXPIRATION_ACCESS,
        email, Role.ADMIN, AUDIENCE_ADMIN
    );
    public static string GenerateAdminRefresh(string email) => Generate(
        TokenType.REFRESH, REFRESH_SECRET, EXPIRATION_REFRESH,
        email, Role.ADMIN, AUDIENCE_ADMIN,
        $"{nameof(Guid)}:{Guid.NewGuid()}"
    );

    public static string GenerateUserAccess(Guid id) => Generate(
        TokenType.ACCESS, ACCESS_SECRET, EXPIRATION_ACCESS,
        id.ToString(), Role.USER, AUDIENCE_USER
    );
    public static string GenerateUserRefresh(Guid id) => Generate(
        TokenType.REFRESH, REFRESH_SECRET, EXPIRATION_REFRESH,
        id.ToString(), Role.USER, AUDIENCE_USER,
        $"{nameof(Guid)}:{Guid.NewGuid()}"
    );

    public static string GenerateActivation(Guid id) => Generate(
        TokenType.ACTIVATION, DATA_SECRET, EXPIRATION_ACTIVATION,
        id.ToString(), Role.USER, AUDIENCE_USER
    );

    public static string GeneratePasswordReset(string email, string password) => Generate(
        TokenType.PASSWORD_RESET, DATA_SECRET, EXPIRATION_PASSWORD_RESET,
        email, Role.USER, AUDIENCE_USER,
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
        return Validate(type, secret, token) ? token : throw Exceptions.NOT_AUTHENTICATED;
    }

    public static bool ValidateAccess(string token) => Validate(TokenType.ACCESS, ACCESS_SECRET, token);
    public static string AssertAccess(string token) => Assert(TokenType.ACCESS, ACCESS_SECRET, token);
    
    public static bool ValidateRefresh(string token) => Validate(TokenType.REFRESH, REFRESH_SECRET, token);
    public static string AssertRefresh(string token) => Assert(TokenType.REFRESH, REFRESH_SECRET, token);
    
    public static bool ValidateActivation(string token) => Validate(TokenType.ACTIVATION, DATA_SECRET, token);
    public static string AssertActivation(string token) => Assert(TokenType.ACTIVATION, DATA_SECRET, token);
    
    public static bool ValidatePasswordReset(string token) => Validate(TokenType.PASSWORD_RESET, DATA_SECRET, token);
    public static string AssertPasswordReset(string token) => Assert(TokenType.PASSWORD_RESET, DATA_SECRET, token);

    // Refresh ----------------------------------------------------------------------------------------------------------------------------
    public static void SetRefreshTokenCookie(string token) {
        var cookieStorage = AppEnvironment.GetService<CookieStorage>();
        var expires = DateTimeOffset.UtcNow.AddMilliseconds(EXPIRATION_REFRESH);
        var cookie = new Client.Models.Cookie(
            Cookies.Mandatory.APP_REFRESH_TOKEN, token,
            expires: expires,
            path: API.BASE.AUTH_REFRESH, httpOnly: true, secure: true
        );
        cookieStorage.Set(cookie);
        cookie = new Client.Models.Cookie(
            Cookies.Mandatory.APP_REFRESH_TOKEN, token,
            expires: expires,
            path: API.BASE.AUTH_INVALIDATE, httpOnly: true, secure: true
        );
        cookieStorage.Set(cookie);
        cookie = new Client.Models.Cookie(
            Cookies.Mandatory.APP_REFRESH_TOKEN, token,
            expires: expires,
            path: API.BASE.AUTH_DELETE, httpOnly: true, secure: true
        );
        cookieStorage.Set(cookie);
    }

    public static void DeleteRefreshTokenCookie() {
        var cookieStorage = AppEnvironment.GetService<CookieStorage>();
        cookieStorage.Delete(Cookies.Mandatory.APP_REFRESH_TOKEN, path: API.BASE.AUTH_REFRESH);
        cookieStorage.Delete(Cookies.Mandatory.APP_REFRESH_TOKEN, path: API.BASE.AUTH_INVALIDATE);
        cookieStorage.Delete(Cookies.Mandatory.APP_REFRESH_TOKEN, path: API.BASE.AUTH_DELETE);
    }

    // Authorization ----------------------------------------------------------------------------------------------------------------------
    public static void Authorize(string token, Role[] roles) {
        // Validate token:
        if (!ValidateAccess(token)) throw Exceptions.NOT_AUTHENTICATED;
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
        if (!allowed) throw Exceptions.NOT_AUTHORIZED;
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
        if (!(!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith($"{AuthTypes.BEARER} "))) throw Exceptions.NOT_AUTHENTICATED;
        string token = authHeader.Substring($"{AuthTypes.BEARER} ".Length).Trim();

        // Authorize:
        Authorize(token, roleAttribute.Allowed);
    }
}
