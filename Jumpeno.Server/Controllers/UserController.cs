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

    // [HttpPost]
    // public async Task Create([FromBody] Person person) {
    //     var db = await DB.Context;
    //     db.Persons.Add(person);
    //     await db.SaveChangesAsync();
    // }

    // [HttpGet]
    // public async Task<List<Person>> Read() {
    //     var db = await DB.Context;
    //     var result = await db.Persons.ToListAsync();
    //     await db.DisposeAsync();
    //     return result;
    // }

    // [HttpPatch]
    // public async Task Update([FromBody] int id) {
    //     var db = await DB.Context;
    //     var person = db.Persons.FirstOrDefault(x => x.Id == id);
    //     if (person != null) {
    //         person.Name = "Igor";
    //         db.Persons.Update(person);
    //         await db.SaveChangesAsync();
    //     } else {
    //         await db.DisposeAsync();
    //     }
    // }

    // [HttpDelete]
    // public async Task Delete([FromBody] int id) {
    //     var db = await DB.Context;
    //     db.Persons.Remove(new() { Id = id });
    //     await db.SaveChangesAsync();
    // }

    
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
        if (!await ActivationEntity.Delete(Token.Activation.sub)) {
            throw Exceptions.InvalidToken;
        }
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
}
