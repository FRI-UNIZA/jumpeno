namespace Jumpeno.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase {
    /// <summary>
    /// Adds a new player to the game.
    /// </summary>
    /// <param name="name">The player object.</param>
    /// <response code="200">If the player is successfully created.</response>
    [HttpGet][Role(ROLE.USER)]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    public object Profile([Required] int name, int? age) {
        foreach (var email in ServerSettings.Auth.Admins) {
            Console.WriteLine($"admin email: {email}");
        }

        return new { Meno = "Fero" };
    }

    [HttpGet("{id}/{bora?}")][Role(ROLE.ADMIN, ROLE.USER)]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    public object Profile([FromRoute] string id, [FromRoute] string? bora, [Required] string name, int? age) {
        foreach (var email in ServerSettings.Auth.Admins) {
            Console.WriteLine($"admin email: {email}");
        }

        return new { Meno = "Fero" };
    }
    
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
            await PasswordEntity.Create(user, body.Password);
            await ActivationEntity.Create(user);
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
        var user = await UserEntity.SelectJoinPassword(body.Email) ?? throw Exceptions.NotAuthenticated;
        if (user.Password == null) throw Exceptions.NotAuthenticated;
        if (!PasswordEntity.Validate(body.Password, user.Password.Salt, user.Password.Hash)) throw Exceptions.NotAuthenticated;
        var id = Guid.Parse(user.ID);
        // 3) Create tokens:
        var accessToken = JWT.GenerateUserAccess(id);
        var refreshToken = JWT.GenerateUserRefresh(id);
        // 4) Store refresh:
        await RefreshEntity.Create(user, refreshToken);
        // 5) Response:
        return new(
            accessToken,
            refreshToken
        );
    }

    /// <summary>Request for password reset token.</summary>
    /// <param name="body">User email.</param>
    /// <response code="200">Reset token generated and sent to email.</response>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> PasswordResetRequest([FromBody] UserPasswordResetRequestDTO body) {
        // 1) Validation:
        body.Check();
        // 2) Authentication:
        var user = await UserEntity.Select(body.Email) ?? throw Exceptions.NotAuthenticated;
        // 3) Generate password:
        var g = new StringGenerator();
        var password = g.Generate(UserValidator.PASSWORD_MIN_LENGTH, UserValidator.PASSWORD_MAX_LENGTH, CHARS.ALPHA_NUM);
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
        return new MessageDTOR(I18N.T("Check your email address."));
    }

    /// <summary>Request for password reset.</summary>
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
        var user = await UserEntity.Select(Token.PasswordReset.sub) ?? throw Exceptions.InvalidToken;
        if (!await PasswordEntity.Update(user.ID, Token.PasswordReset.data)) throw Exceptions.InvalidToken;
        // 3) Response:
        return new(I18N.T("Password reset successful."));
    }
}
