namespace Jumpeno.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class UserController : ControllerBase {
    [HttpPost]
    [ProducesResponseType(typeof(LogInAdminDTOR), StatusCodes.Status200OK)]
    public LogInAdminDTOR LogInAdmin([FromBody] LogInAdminDTO data) {
        // 1) Validation:
        data.Check();
        // 2) Authentication:
        string? email = null;
        foreach (var adminEmail in ServerSettings.Admins) {
            if (data.Email == adminEmail) { email = adminEmail; break; } 
        }
        if (email == null) throw Exceptions.NotAuthenticated;
        // 3) Send token:
        var q = new QueryParams(); q.Set(nameof(AccessToken), JWT.GenerateAdmin(email));
        var link = URL.ToAbsolute(URL.SetQueryParams(I18N.Link<LoginPage>(), q));
        
        var theme = ThemeProvider.DEFAULT_THEME;
        var text = $"<h1>Jumpeno login</h1>";
        text += $"<p>{I18N.T("Hello, here is your login link")}:</p>";
        text += $"<a href=\"{link}\" class=\"login-link\">{I18N.T("Log in")}</a>";
        text += "<style>";
        text += $"h1 {{ font-family: {theme.FONT_PRIMARY}; font-size: 20px; margin-bottom: 16px; }}";
        text += $"p {{ font-family: {theme.FONT_PRIMARY}; font-size: 14px; margin: 0; }}";
        text += ".login-link {";
        text += "display: inline-flex; padding: 12px 16px; border-radius: 100px;";
        text += $"background-color: rgb({theme.COLOR_BASE}); color: rgb({theme.COLOR_BASE_INVERT}); cursor: pointer;";
        text += $"font-family: {theme.FONT_PRIMARY}; font-size: 14px; font-weight: bold; text-decoration: none;";
        text += "margin-top: 16px;";
        text += "transition: background-color 200ms;";
        text += "}";
        text += ".login-link:hover {";
        text += $"background-color: rgb({theme.COLOR_BASE_HIGHLIGHT})";
        text += "}";
        text += "</style>";

        Email.Send(email, "Jumpeno login", text);
        // 4) Send response:
        return new LogInAdminDTOR(I18N.T("Token was sent to your email."));
    }

    [HttpPost]
    public void LogInUser([FromBody] LogInUserDTO data) {
        
    }

    [HttpGet][Role(ROLE.USER, ROLE.ADMIN)]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    public object ProfileAdmin() {
        foreach (var email in ServerSettings.Admins) {
            Console.WriteLine($"admin email: {email}");
        }

        return new { Meno = "Fero" };
    }

    /// <summary>
    /// Adds a new player to the game.
    /// </summary>
    /// <param name="name">The player object.</param>
    /// <response code="200">If the player is successfully created.</response>
    [HttpGet][Role(ROLE.ADMIN, ROLE.USER)]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    public object Profile([Required] int name, int? age) {
        foreach (var email in ServerSettings.Admins) {
            Console.WriteLine($"admin email: {email}");
        }

        return new { Meno = "Fero" };
    }

    [HttpGet("{id}/{bora?}")][Role(ROLE.ADMIN, ROLE.USER)]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    public object Profile([FromRoute] string id, [FromRoute] string? bora, [Required] string name, int? age) {
        foreach (var email in ServerSettings.Admins) {
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

    [HttpPost]
    public async Task Register([FromBody] UserRegisterDTO data) {
        var db = await DB.Context();
        await EntityUser.Add(data);
        await db.SaveChangesAsync();
    }
}
