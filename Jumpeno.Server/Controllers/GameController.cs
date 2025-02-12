namespace Jumpeno.Server.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class GameController : ControllerBase {
    [HttpPatch(Name = "PatchGameStart")]
    public async Task Start([FromBody] string code) {
        await GameService.StartGame(code);
    }

    [HttpPatch(Name = "PatchGamePause")]
    public async Task Pause([FromBody] string code) {
        await GameService.PauseGame(code);
    }

    [HttpPatch(Name = "PatchGameResume")]
    public async Task Resume([FromBody] string code) {
        await GameService.ResumeGame(code);
    }

    [HttpPatch(Name = "PatchGameReset")]
    public async Task Reset([FromBody] string code) {
        await GameService.ResetGame(code);
    }
}
