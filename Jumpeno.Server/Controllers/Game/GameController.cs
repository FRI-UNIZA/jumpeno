namespace Jumpeno.Server.Controllers;

[ApiController]
[Microsoft.AspNetCore.Mvc.Route("[controller]/[action]")]
public class GameController : ControllerBase {
    /// <summary>Returns list of available game map identifiers.</summary>
    /// <response code="200">List of map identifiers.</response>
    [HttpGet]
    [ProducesResponseType(typeof(GameMapsDTOR), StatusCodes.Status200OK)]
    public GameMapsDTOR Maps()
    {
        // 1) Select maps:
        var maps = MAP.List();
        // 2) Create DTO:
        GameMapsDTOR result = new([]);
        for (int i = 0; i < maps.Count; i++)
        {
            result.Maps.Add(new(i, maps[i].Name));
        }
        // 3) Return result:
        return result;
    }

    /// <summary>Returns selected game map model.</summary>
    /// <param name="query">Map data.</param>
    /// <response code="200">Game map.</response>
    [HttpGet]
    [ProducesResponseType(typeof(GameMapDTOR), StatusCodes.Status200OK)]
    public GameMapDTOR Map([FromQuery] GameMapDTO query)
    {
        // 1) Read query params:
        var q = query?.Assert() ?? throw EXCEPTION.VALUES.Add(ERROR.EMPTY);
        // 2) Select map:
        var map = MAP.ByID(q.ID, nameof(GameMapDTO.ID));
        // 3) Return result:
        return new(map);
    }

    /// <summary>Starts or resumes paused game.</summary>
    /// <param name="body">Game control data.</param>
    /// <response code="200">Game is running.</response>
    [HttpPatch][Role(ROLE.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task Start([FromBody] GameControlDTO body) => await GameService.StartGame(body.Assert().Code, nameof(GameControlDTO.Code));

    /// <summary>Pauses the game.</summary>
    /// <param name="body">Game control data.</param>
    /// <response code="200">Game is paused.</response>
    [HttpPatch][Role(ROLE.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task Pause([FromBody] GameControlDTO body) => await GameService.PauseGame(body.Assert().Code, nameof(GameControlDTO.Code));

    /// <summary>Starts, resumes or pauses the game based on its current state.</summary>
    /// <param name="body">Game control data.</param>
    /// <response code="200">Game state updated.</response>
    [HttpPatch][Role(ROLE.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task Toggle([FromBody] GameControlDTO body) => await GameService.ToggleGame(body.Assert().Code, nameof(GameControlDTO.Code));

    /// <summary>Deletes the game.</summary>
    /// <param name="body">Game control data.</param>
    /// <response code="200">Game is deleted.</response>
    [HttpPatch][Role(ROLE.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task Delete([FromBody] GameControlDTO body) => await GameService.DeleteGame(body.Assert().Code, nameof(GameControlDTO.Code));
}
