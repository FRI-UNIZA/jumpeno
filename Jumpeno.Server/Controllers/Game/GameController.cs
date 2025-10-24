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
        result.Maps.Add(new(100, "Jumper's home"));
        result.Maps.Add(new(201, "100 Needles"));
        result.Maps.Add(new(302, "Magic temple"));
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
        // 2) Generate map:
        Random rand = new();
        List<Tile> tiles = [];
        for (int i = 0; i < 10; i++) {
            tiles.Add(new(new(rand.Next() % 16 * Tile.SIZE + Tile.HALF_SIZE, rand.Next() % 9 * Tile.SIZE + Tile.HALF_SIZE)));
        }
        return new(
            new(
                Client.Models.Map.DEFAULT_NAME,
                tiles,
                new((byte)(rand.Next() % 256), (byte)(rand.Next() % 256), (byte)(rand.Next() % 256)),
                new((byte)(rand.Next() % 256), (byte)(rand.Next() % 256), (byte)(rand.Next() % 256)),
                new((byte)(rand.Next() % 256), (byte)(rand.Next() % 256), (byte)(rand.Next() % 256))
            )
        );
    }
}
