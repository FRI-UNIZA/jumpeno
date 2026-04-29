namespace Jumpeno.Server.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]/[action]")]
public class UserController (CaptchaValidatorService captchaService) : ControllerBase {
    /// <summary>New user account registration.</summary>
    /// <param name="body">Registration data.</param>
    /// <response code="201">User is successfully registered.</response>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status201Created)]
    public async Task<MessageDTOR> Register([FromBody] UserRegisterDTO body) {
        // 1) Validation:
        body.Assert();
        await captchaService.AssertTokenForIP(AttemptsCategory.Register, body.CAPTCHAToken, nameof(UserRegisterDTO.CAPTCHAToken));
        // 2) Transaction:
        UserEntity user = null!;
        await DB.Transaction(async () => {
            user = await UserEntity.Create(body.Email.ToLower(), body.Name, nameof(body.Email), nameof(body.Name));
            await PasswordEntity.Create(user.Id, body.Password, passwordID: nameof(body.Password));
            await ActivationEntity.Create(user.Id);
        });
        // 3) Activation email:
        Services.Email.TrySendActivation(user.Email, user.Id);
        // 4) Response:
        Response.StatusCode = StatusCodes.Status201Created;
        return new(I18N.T("Registration successful."));
    }

    /// <summary>Sends activation email to authenticated user.</summary>
    /// <response code="200">Activation email sent.</response>
    [HttpPost][Role(Role.User)]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> SendActivation() {
        // 1) Select user:
        var user = await UserEntity.ByIDLeftJoinActivation(Token.Access.sub) ?? throw Exceptions.NotAuthenticated;
        // 2) Check activation:
        if (user.Activation == null) throw Exceptions.NotFound.SetInfo("Account already activated.");
        // 3) Activation email:
        Services.Email.SendActivation(user.Email, user.Id);
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
            throw Exceptions.InvalidToken;
        }
        // 2) Activation:
        if (!await ActivationEntity.Delete(Token.Activation.sub, nameof(body.ActivationToken))) throw Exceptions.InvalidToken;
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
        await captchaService.AssertTokenForEmailAndIP(body.CAPTCHAToken, body.Email, AttemptsCategory.Login, nameof(UserLoginDTO.CAPTCHAToken));
        // 2) Authentication:
        var user = await UserEntity.ByEmailLeftJoinPassword(body.Email, nameof(body.Email)) ?? throw Exceptions.NotAuthenticated;
        if (user.Password == null) throw Exceptions.NotAuthenticated;
        if (!PasswordEntity.Validate(body.Password, user.Password.Salt, user.Password.Hash)) throw Exceptions.NotAuthenticated;
        // 3) Create tokens:
        var id = Guid.Parse(user.Id);
        var accessToken = JWT.GenerateUserAccess(id);
        var refreshToken = JWT.GenerateUserRefresh(id);
        // 4) Store refresh:
        await RefreshEntity.Create(refreshToken, user.Id);
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
        var user = await UserEntity.ByEmail(body.Email, nameof(body.Email)) ?? throw Exceptions.NotAuthenticated;
        // 3) Generate password:
        var g = new StringGenerator();
        var password = g.GenerateResetPassword(UserValidator.PasswordGeneratorMinLength, UserValidator.PasswordGeneratorMaxLength);
        // 4) Send email:
        Services.Email.SendPasswordReset(user.Email, password, JWT.GeneratePasswordReset(user.Email, password));
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
            throw Exceptions.InvalidToken;
        }
        // 2) Password reset:
        var user = await UserEntity.ByEmail(Token.PasswordReset.sub, nameof(body.ResetToken)) ?? throw Exceptions.InvalidToken;
        if (!await PasswordEntity.Update(user.Id, Token.PasswordReset.data, nameof(body.ResetToken), nameof(body.ResetToken))) throw Exceptions.InvalidToken;
        // 3) Response:
        return new(I18N.T("Password reset successful."));
    }

    /// <summary>Changes authenticated user password.</summary>
    /// <response code="200">Password changed.</response>
    [HttpPatch][Role(Role.User)]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> PasswordChange([FromBody] UserPasswordChangeDTO body)
    {
        // 1) Validation:
        body.Assert();
        // 2) Change password:
        await DB.Transaction(
            async () =>
            {
                var existing = await PasswordEntity.ByIDLeftJoinRefresh(Token.Access.sub);
                if (existing != null)
                {
                    if (!await PasswordEntity.Update(existing.Value.Item1.ID, body.NewPassword, passwordID: nameof(body.NewPassword))) throw Exceptions.Default;
                }
                else
                {
                    await PasswordEntity.Create(Token.Access.sub, body.NewPassword, passwordID: nameof(body.NewPassword));
                }
                await RefreshEntity.DeleteByUserID(Token.Access.sub);
            },
            Isolation.Serializable
        );
        // 3) Response:
        return new(I18N.T("Password has been changed."));
    }

    /// <summary>User profile info.</summary>
    /// <response code="200">User profile.</response>
    [HttpGet][Role(Role.User)]
    [ProducesResponseType(typeof(UserProfileDTOR), StatusCodes.Status200OK)]
    public async Task<UserProfileDTOR> Profile() {
        // 1) Select user:
        var user = await UserEntity.ByIDLeftJoinActivation(Token.Access.sub) ?? throw Exceptions.NotAuthenticated;
        // 2) Cast to profile:
        var profile = new User(Guid.Parse(user.Id), user.Email, user.Name, (Skin)user.Skin, user.Activation == null);
        // 3) Response:
        return new(profile);
    }

    /// <summary>Updates authenticated user data.</summary>
    /// <response code="200">User data updated.</response>
    [HttpPatch][Role(Role.User)]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> Update([FromBody] UserUpdateDTO body)
    {
        // 1) Validation:
        body.Assert();
        // 2) Update data:
        if (!await UserEntity.Modify(Token.Access.sub, 
            name: body.NewName, nameId: nameof(UserUpdateDTO.NewName),
            skin: body.NewSkin, skinId: nameof(UserUpdateDTO.NewSkin),
            email: body.NewEmail, emailId: nameof(UserUpdateDTO.NewEmail)
        )) throw Exceptions.Default;
        // 3) Response:
        return new(I18N.T("User data has been updated."));
    }

    /// <summary>Deletes authenticated user account.</summary>
    /// <response code="200">Account deleted.</response>
    [HttpDelete][Role(Role.User)]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public async Task<MessageDTOR> Delete() {
        // 1) Delete User:
        if(!await UserEntity.Delete(Token.Access.sub)) throw Exceptions.Default;
        // 2) Response:
        return new(I18N.T("Account deleted."));
    }
}
