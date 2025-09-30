namespace Jumpeno.Server.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]/[action]")]
public class AuthController : ControllerBase {
    /// <summary>Refresh of tokens.</summary>
    /// <param name="body">Refresh token. (can be set as cookie)</param>
    /// <response code="200">Extended tokens.</response>
    /// <response code="406">Invalid token.</response>
    [HttpPost]
    [ProducesResponseType(typeof(AuthRefreshDTOR), StatusCodes.Status200OK)]
    public async Task<AuthRefreshDTOR> Refresh([FromBody] AuthRefreshDTO? body) {
        // 1) Validation:
        try { body?.Assert(); } catch { throw EXCEPTION.INVALID_TOKEN; }
        var token = body?.RefreshToken ?? CookieStorage.Get(COOKIE.MANDATORY.APP_REFRESH_TOKEN) ?? throw EXCEPTION.INVALID_TOKEN;
        try {
            JWT.AssertRefresh(token);
            Token.StoreRefresh(token);
        } catch {
            throw EXCEPTION.INVALID_TOKEN;
        }
        if (!await RefreshEntity.IsValid(token, nameof(body.RefreshToken))) throw EXCEPTION.INVALID_TOKEN;
        // 2) Create new tokens:
        string accessToken;
        string refreshToken;
        if (Token.Refresh.role == ROLE.USER) {
            var id = Guid.Parse(Token.Refresh.sub);
            accessToken = JWT.GenerateUserAccess(id);
            refreshToken = JWT.GenerateUserRefresh(id);
            await RefreshEntity.Create(refreshToken, Token.Refresh.sub, token);
        } else {
            var email = Token.Refresh.sub;
            accessToken = JWT.GenerateAdminAccess(email);
            refreshToken = JWT.GenerateAdminRefresh(email);
            await RefreshEntity.Create(refreshToken, null, token);
        }
        // 3) Set cookie:
        JWT.SetRefreshTokenCookie(refreshToken);
        // 4) Response:
        return new(
            accessToken,
            refreshToken
        );
    }

    /// <summary>Invalidation of refresh token origin.</summary>
    /// <param name="body">Refresh token. (can be set as cookie)</param>
    /// <response code="200">Origin and other successors invalidated.</response>
    /// <response code="406">Invalid token.</response>
    [HttpDelete]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> Invalidate([FromBody] AuthInvalidateDTO? body) {
        // 1) Validation:
        try { body?.Assert(); } catch { throw EXCEPTION.INVALID_TOKEN; }
        var token = body?.RefreshToken ?? CookieStorage.Get(COOKIE.MANDATORY.APP_REFRESH_TOKEN) ?? throw EXCEPTION.INVALID_TOKEN;
        try { JWT.AssertRefresh(token); } catch { throw EXCEPTION.INVALID_TOKEN; }
        // 2) Delete origin with successors:
        await DB.Transaction(async () => {
            var refresh = await RefreshEntity.ByToken(token, nameof(body.RefreshToken)) ?? throw EXCEPTION.INVALID_TOKEN;
            if (refresh.Origin == null) return;
            await RefreshEntity.Delete(refresh.Origin, nameof(body.RefreshToken));
            await RefreshEntity.DeleteByOrigin(refresh.Origin, token, nameof(body.RefreshToken), nameof(body.RefreshToken));
        });
        // 3) Response:
        return new(I18N.T("Token invalidated."));
    }

    /// <summary>Deletes refresh token.</summary>
    /// <param name="body">Refresh token. (can be set as cookie)</param>
    /// <response code="200">Refresh token deleted.</response>
    /// <response code="406">Invalid token.</response>
    [HttpDelete]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> Delete([FromBody] AuthDeleteDTO? body) {
        // 1) Validation:
        try { body?.Assert(); } catch { throw EXCEPTION.INVALID_TOKEN; }
        var token = body?.RefreshToken ?? CookieStorage.Get(COOKIE.MANDATORY.APP_REFRESH_TOKEN) ?? throw EXCEPTION.INVALID_TOKEN;
        try { JWT.AssertRefresh(token); } catch { throw EXCEPTION.INVALID_TOKEN; }
        // 2) Delete refresh token:
        await RefreshEntity.Delete(token, nameof(body.RefreshToken));
        // 3) Delete cookie:
        JWT.DeleteRefreshTokenCookie();
        // 4) Response:
        return new(I18N.T("Token deleted."));
    }
}
