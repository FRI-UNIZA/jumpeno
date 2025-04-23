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
        body.Check();
        // 2) Transaction:
        UserEntity user = null!;
        await DB.Transaction(async () => {
            user = await UserEntity.Create(body.Email, body.Name);
            await PasswordEntity.Create(user.ID, body.Password);
            await ActivationEntity.Create(user.ID);
        });
        // 3) Activation email:
        var q = new QueryParams(); q.Set(TOKEN_TYPE.ACTIVATION.String(), JWT.GenerateActivation(Guid.Parse(user.ID)));
        Email.TrySend(
            user.Email,
            I18N.T("Jumpeno activation"), 
            Email.LINK_CONTENT(
                I18N.T("Jumpeno activation"),
                I18N.T("Hello, here is your activation link:"),
                I18N.T("Activate"),
                URL.ToAbsolute(URL.SetQueryParams(I18N.Link<HomePage>(), q))
            )
        );
        // 4) Response:
        Response.StatusCode = StatusCodes.Status201Created;
        return new(I18N.T("Registration successful."));
    }

    /// <summary>Activation of existing user account.</summary>
    /// <param name="body">Activation token.</param>
    /// <response code="200">User is successfully activated.</response>
    [HttpPatch]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> Activate([FromBody] UserActivateDTO body) {
        // 1) Validation:
        try {
            body.Check();
            JWT.CheckActivation(body.ActivationToken);
            Token.StoreActivation(body.ActivationToken);
        } catch {
            throw Exceptions.InvalidToken;
        }
        // 2) Activation:
        if (!await ActivationEntity.Delete(Token.Activation.sub)) throw Exceptions.InvalidToken;
        // 3) Response:
        return new(I18N.T("Account activated."));
    }

    /// <summary>User login.</summary>
    /// <param name="body">User email and password.</param>
    /// <response code="200">User is logged in.</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserLoginDTOR), StatusCodes.Status200OK)]
    public async Task<UserLoginDTOR> Login([FromBody] UserLoginDTO body) {
        // 1) Validation:
        body.Check();
        // 2) Authentication:
        var user = await UserEntity.ByEmailLeftJoinPassword(body.Email) ?? throw Exceptions.NotAuthenticated;
        if (user.Password == null) throw Exceptions.NotAuthenticated;
        if (!PasswordEntity.Validate(body.Password, user.Password.Salt, user.Password.Hash)) throw Exceptions.NotAuthenticated;
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
        body.Check();
        // 2) Authentication:
        var user = await UserEntity.ByEmail(body.Email) ?? throw Exceptions.NotAuthenticated;
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
            body.Check();
            JWT.CheckPasswordReset(body.ResetToken);
            Token.StorePasswordReset(body.ResetToken);
        } catch {
            throw Exceptions.InvalidToken;
        }
        // 2) Password reset:
        var user = await UserEntity.ByEmail(Token.PasswordReset.sub) ?? throw Exceptions.InvalidToken;
        if (!await PasswordEntity.Update(user.ID, Token.PasswordReset.data)) throw Exceptions.InvalidToken;
        // 3) Response:
        return new(I18N.T("Password reset successful."));
    }

    /// <summary>User profile info.</summary>
    /// <response code="200">User profile.</response>
    [HttpGet][Role(ROLE.USER)]
    [ProducesResponseType(typeof(UserProfileDTOR), StatusCodes.Status200OK)]
    public async Task<UserProfileDTOR> Profile() {
        // 1) Select user:
        var user = await UserEntity.ByIDLeftJoinActivation(Token.Access.sub) ?? throw Exceptions.NotAuthenticated;
        // 2) Cast to profile:
        var profile = new User(Guid.Parse(user.ID), user.Email, user.Name, (SKIN)user.Skin, user.Activation == null);
        // 3) Response:
        return new(profile);
    }
}
