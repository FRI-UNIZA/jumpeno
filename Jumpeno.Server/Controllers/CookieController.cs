namespace Jumpeno.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class CookieController : ControllerBase {
    [HttpPatch(Name = "CookiePatch")]
    public void Patch([FromBody] List<string> acceptedNames) {
        CookieStorage.AcceptCookieConsent(CookieStorage.ConvertToTypes(acceptedNames));
    }
}
