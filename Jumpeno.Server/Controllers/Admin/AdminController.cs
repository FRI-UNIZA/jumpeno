namespace Jumpeno.Server.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]/[action]")]
public class AdminController : ControllerBase {
    /// <summary>Sends admin refresh token to email.</summary>
    /// <param name="body">Admin email.</param>
    /// <response code="200">Refresh token generated and sent to email.</response>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> Login([FromBody] AdminLoginDTO body) {
        // 1) Validation:
        body.Assert();
        // 2) Authentication:
        string? email = null;
        foreach (var adminEmail in ServerSettings.Auth.Admins) {
            if (body.Email.Equals(adminEmail, StringComparison.CurrentCultureIgnoreCase)) {
                email = adminEmail; break;
            }
        }
        if (email == null) throw Exceptions.NotAuthenticated;
        // 3) Create refresh token:
        var refreshToken = JWT.GenerateAdminRefresh(email);
        // 4) Store refresh token:
        await RefreshEntity.Create(refreshToken);
        // 5) Send token:
        Services.Email.SendAdminLogin(email, refreshToken);
        // 6) Send response:
        return new(I18N.T("Token was sent to your email."));
    }

    /// <summary>Returns database credentials.</summary>
    /// <response code="200">Database credentials.</response>
    [HttpGet][Role(Role.Admin)]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public MessageDTOR DBCredentials() => new(ServerSettings.Database.ConnectionString);

    /// <summary>Returns email password.</summary>
    /// <response code="200">Email password.</response>
    [HttpGet][Role(Role.Admin)]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public MessageDTOR EmailPassword() => new(ServerSettings.Email.Password);
    
    /// <summary>Returns email backup keys.</summary>
    /// <response code="200">Email backup keys.</response>
    [HttpGet][Role(Role.Admin)]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public MessageDTOR EmailBackupKeys() => new(ServerSettings.Email.BackupKeys);
}
