namespace Jumpeno.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class CookieController : ControllerBase {
    /// <summary>Sets accepted cookies.</summary>
    /// <param name="body">List of accepted cookie names.</param>
    /// <response code="200">Cookies accepted.</response>
    [HttpPatch]
    [ProducesResponseType(typeof(MessageDTOR), StatusCodes.Status200OK)]
    public MessageDTOR Set([FromBody] CookieSetDTO body) {
        // 1) Validation:
        body.Validate();
        // 2) Set cookies:
        CookieStorage.AcceptCookieConsent(body.AcceptedNames);
        // 3) Response:
        return new(I18N.T("Cookies accepted."));
    }
}
