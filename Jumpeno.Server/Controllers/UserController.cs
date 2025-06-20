namespace Jumpeno.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase {
    /// <summary>New user account registration.</summary>
    /// <param name="body">Registration data.</param>
    /// <response code="201">User is successfully registered.</response>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status201Created)]
    public async Task<MessageDTOR> Register([FromBody] UserRegisterDTO body) {
        // 1) Validation:
        body.Assert();
        // 2) Transaction:
        UserEntity user = null!;
        await DB.Transaction(async () => {
            user = await UserEntity.Create(body.Email.ToLower(), body.Name, nameof(body.Email), nameof(body.Name));
            await PasswordEntity.Create(user.ID, body.Password, passwordID: nameof(body.Password));
            await ActivationEntity.Create(user.ID);
        });
        // 3) Activation email:
        Email.TrySendActivation(user.Email, user.ID);
        // 4) Response:
        Response.StatusCode = StatusCodes.Status201Created;
        return new(I18N.T("Registration successful."));
    }

    /// <summary>Sends activation email to authenticated user.</summary>
    /// <response code="200">Activation email sent.</response>
    [HttpPost][Role(ROLE.USER)]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> SendActivation() {
        // 1) Select user:
        var user = await UserEntity.ByIDLeftJoinActivation(Token.Access.sub) ?? throw EXCEPTION.NOT_AUTHENTICATED;
        // 2) Check activation:
        if (user.Activation == null) throw EXCEPTION.NOT_FOUND.SetInfo("Account already activated.");
        // 3) Activation email:
        Email.SendActivation(user.Email, user.ID);
        // 4) Response:
        return new(I18N.T("Activation email sent."));
    }

    /// <summary>Activation of existing user account.</summary>
    /// <param name="body">Activation token.</param>
    /// <response code="200">User is successfully activated.</response>
    [HttpPatch]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> Activate([FromBody] UserActivateDTO body) {
        // 1) Validation:
        try {
            body.Assert();
            JWT.AssertActivation(body.ActivationToken);
            Token.StoreActivation(body.ActivationToken);
        } catch {
            throw EXCEPTION.INVALID_TOKEN;
        }
        // 2) Activation:
        if (!await ActivationEntity.Delete(Token.Activation.sub, nameof(body.ActivationToken))) throw EXCEPTION.INVALID_TOKEN;
        // 3) Response:
        return new($"{I18N.T("Account activated")}.");
    }

    /// <summary>User login.</summary>
    /// <param name="body">User email and password.</param>
    /// <response code="200">User is logged in.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserLoginDTOR), StatusCodes.Status200OK)]
    public async Task<UserLoginDTOR> Login([FromBody] UserLoginDTO body) {
        // 1) Validation:
        body.Assert();
        // 2) Authentication:
        var user = await UserEntity.ByEmailLeftJoinPassword(body.Email, nameof(body.Email)) ?? throw EXCEPTION.NOT_AUTHENTICATED;
        if (user.Password == null) throw EXCEPTION.NOT_AUTHENTICATED;
        if (!PasswordEntity.Validate(body.Password, user.Password.Salt, user.Password.Hash)) throw EXCEPTION.NOT_AUTHENTICATED;
        // 3) Create tokens:
        var id = Guid.Parse(user.ID);
        var accessToken = JWT.GenerateUserAccess(id);
        var refreshToken = JWT.GenerateUserRefresh(id);
        // 4) Store refresh:
        await RefreshEntity.Create(refreshToken, user.ID);
        // 5) Set cookie:
        JWT.SetRefreshTokenCookie(refreshToken);
        // 6) Response:
        return new(
            accessToken,
            refreshToken
        );
    }

    /// <summary>Sends password reset token.</summary>
    /// <param name="body">User email.</param>
    /// <response code="200">Reset token generated and sent to email.</response>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> PasswordResetRequest([FromBody] UserPasswordResetRequestDTO body) {
        // 1) Validation:
        body.Assert();
        // 2) Authentication:
        var user = await UserEntity.ByEmail(body.Email, nameof(body.Email)) ?? throw EXCEPTION.NOT_AUTHENTICATED;
        // 3) Generate password:
        var g = new StringGenerator();
        var password = g.Generate(UserValidator.PASSWORD_GENERATOR_MIN_LENGTH, UserValidator.PASSWORD_GENERATOR_MAX_LENGTH, CHARS.ALPHA_NUM);
        // 4) Send email:
        var q = new QueryParams(); q.Set(TOKEN_TYPE.PASSWORD_RESET.String(), JWT.GeneratePasswordReset(user.Email, password));
        Email.Send(
            user.Email,
            I18N.T("Jumpeno password reset"),
            Email.LINK_CONTENT(
                I18N.T("Jumpeno password reset"),
                $"{I18N.T("Hello, confirm that your password can be reset to:")}"
                + "<br><br>"
                + $"<b>{password}</b>",
                I18N.T("Confirm reset"),
                URL.ToAbsolute(URL.SetQueryParams(I18N.Link<LoginPage>(), q))
            )
        );
        // 5) Send response:
        return new(I18N.T("Check your email address."));
    }

    /// <summary>Resets user password.</summary>
    /// <param name="body">Password reset token.</param>
    /// <response code="200">Password reset successful.</response>
    [HttpPatch]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> PasswordReset([FromBody] UserPasswordResetDTO body) {
        // 1) Validation:
        try {
            body.Assert();
            JWT.AssertPasswordReset(body.ResetToken);
            Token.StorePasswordReset(body.ResetToken);
        } catch {
            throw EXCEPTION.INVALID_TOKEN;
        }
        // 2) Password reset:
        var user = await UserEntity.ByEmail(Token.PasswordReset.sub, nameof(body.ResetToken)) ?? throw EXCEPTION.INVALID_TOKEN;
        if (!await PasswordEntity.Update(user.ID, Token.PasswordReset.data, nameof(body.ResetToken), nameof(body.ResetToken))) throw EXCEPTION.INVALID_TOKEN;
        // 3) Response:
        return new(I18N.T("Password reset successful."));
    }

    /// <summary>User profile info.</summary>
    /// <response code="200">User profile.</response>
    [HttpGet][Role(ROLE.USER)]
    [ProducesResponseType(typeof(UserProfileDTOR), StatusCodes.Status200OK)]
    public async Task<UserProfileDTOR> Profile() {
        // 1) Select user:
        var user = await UserEntity.ByIDLeftJoinActivation(Token.Access.sub) ?? throw EXCEPTION.NOT_AUTHENTICATED;
        // 2) Cast to profile:
        var profile = new User(Guid.Parse(user.ID), user.Email, user.Name, (SKIN)user.Skin, user.Activation == null);
        // 3) Response:
        return new(profile);
    }
}
