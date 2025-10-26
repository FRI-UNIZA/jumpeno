using Jumpeno.Client.Services.Game.Models.Map.Constants;

namespace Jumpeno.Server.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]/[action]")]
public class GameController : ControllerBase {
    [HttpPatch]
    public async Task Start([FromBody] string code) => await GameService.StartGame(code);

    [HttpPatch]
    public async Task Pause([FromBody] string code) => await GameService.PauseGame(code);

    [HttpPatch]
    public async Task Resume([FromBody] string code) => await GameService.ResumeGame(code);

    [HttpPatch]
    public async Task Reset([FromBody] string code) => await GameService.ResetGame(code);

    /// <summary>Returns list of available game map identifiers.</summary>
    /// <response code="200">List of map identifiers.</response>
    [HttpGet]
    [ProducesResponseType(typeof(GameMapsDTOR), StatusCodes.Status200OK)]
    public GameMapsDTOR Maps() {
        GameMapsDTOR result = new([]);
        int i = 0;
        foreach (var map in MAPS.ALL_MAPS)
        {
            result.Maps.Add(new (i, map.Name));
            i++;
        }
        return result;
    }

    /// <summary>Returns selected game map model.</summary>
    /// <param name="query">Map data.</param>
    /// <response code="200">Game map.</response>
    [HttpGet]
    [ProducesResponseType(typeof(GameMapDTOR), StatusCodes.Status200OK)]
    public GameMapDTOR Map([FromQuery] GameMapDTO query) {
        // 1) Read query params:
        var q = query?.Assert() ?? throw EXCEPTION.VALUES.Add(ERROR.EMPTY);
        // 2) Get map:
        var selectedMap = MAPS.ALL_MAPS.ElementAt(q.ID);
        return new(selectedMap);
    }
}
