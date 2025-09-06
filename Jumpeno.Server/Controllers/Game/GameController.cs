namespace Jumpeno.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class GameController : ControllerBase {
    [HttpPatch]
    public async Task Start([FromBody] string code) {
        await GameService.StartGame(code);
    }

    [HttpPatch]
    public async Task Pause([FromBody] string code) {
        await GameService.PauseGame(code);
    }

    [HttpPatch]
    public async Task Resume([FromBody] string code) {
        await GameService.ResumeGame(code);
    }

    [HttpPatch]
    public async Task Reset([FromBody] string code) {
        await GameService.ResetGame(code);
    }
}
