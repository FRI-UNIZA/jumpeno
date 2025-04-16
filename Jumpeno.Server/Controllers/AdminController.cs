namespace Jumpeno.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AdminController : ControllerBase {
    /// <summary>Sends admin refresh token to email.</summary>
    /// <param name="body">Admin email.</param>
    /// <response code="200">Refresh token generated and sent to email.</response>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> Login([FromBody] AdminLoginDTO body) {
        // 1) Validation:
        body.Check();
        // 2) Authentication:
        string? email = null;
        foreach (var adminEmail in ServerSettings.Auth.Admins) {
            if (body.Email == adminEmail) { email = adminEmail; break; } 
        }
        if (email == null) throw Exceptions.NotAuthenticated;
        // 3) Create refresh token:
        var refreshToken = JWT.GenerateAdminRefresh(email);
        // 4) Store refresh token:
        await RefreshEntity.Create(refreshToken);
        // 5) Send token:
        var q = new QueryParams(); q.Set(TOKEN_TYPE.REFRESH.String(), refreshToken);
        Email.Send(
            email,
            I18N.T("Jumpeno login"),
            Email.LINK_CONTENT(
                I18N.T("Jumpeno login"),
                I18N.T("Hello, here is your login link:"),
                I18N.T("Log in"),
                URL.ToAbsolute(URL.SetQueryParams(I18N.Link<LoginPage>(), q))
            )
        );
        // 6) Send response:
        return new(I18N.T("Token was sent to your email."));
    }
}
