namespace Jumpeno.Client.Constants;

public class MAPS
{
    // Colors -----------------------------------------------------------------------------------------------------------------------------
    private static readonly RGBColor COLOR_WHITE = new(255, 255, 255);
    private static readonly RGBColor COLOR_BLACK = new(10, 10, 10);
    private static readonly RGBColor COLOR_DARK = new(36, 30, 59);
    private static readonly RGBColor COLOR_YELLOW = new(255, 255, 0);
    private static readonly RGBColor COLOR_DARK_PURPLE = new(51, 0, 51);
    private static readonly RGBColor COLOR_DARK_GREEN = new(7, 30, 24);
    private static readonly RGBColor COLOR_DARK_BLUE = new(3, 14, 22);

    // Maps definitions -------------------------------------------------------------------------------------------------------------------
    // Jumper's home:
    private static readonly Map MAP_JUMPERS_HOME = new(
        "Jumper's home", Tile.CreateTiles(JUMPERS_HOME_TILE_POSITIONS),
        COLOR_DARK, IMAGE.MAP_JUMPERS_HOME_TILE, COLOR_YELLOW, COLOR_WHITE, COLOR_BLACK
    );
    private static List<(int x, int y)> JUMPERS_HOME_TILE_POSITIONS =>
    [
        (1, 0), (2, 0), (10, 0), (12, 0),
        (12, 1),
        (5, 2), (6, 2), (7, 2), (8, 2), (9, 2),
        (12, 7), (12, 8), (13, 0)
    ];

    // Magic Temple:
    private static readonly Map MAP_MAGIC_TEMPLE = new(
        "Magic Temple", Tile.CreateTiles(MAGIC_TEMPLE_TILE_POSITIONS),
        COLOR_DARK_PURPLE, IMAGE.MAP_MAGIC_TEMPLE_BACKGROUND, IMAGE.MAP_MAGIC_TEMPLE_TILE, COLOR_WHITE, COLOR_WHITE, COLOR_DARK_PURPLE
    );
    private static List<(int x, int y)> MAGIC_TEMPLE_TILE_POSITIONS =>
    [
        (0, 0), (1, 0), (2, 0), (8, 0), (12, 0), (13, 0), (14, 0), (15, 0),
        (0, 1), (8, 1), (15, 1),
        (15, 2),
        (2, 3), (3, 3), (4, 3), (10, 3),
        (4, 4), (5, 4), (11, 4), (13, 4),
        (5, 5), (6, 5), (8, 5)
    ];

    // Emerald Grove:
    private static readonly Map MAP_EMERALD_GROVE = new(
        "Emerald Grove", Tile.CreateTiles(EMERALD_GROVE_TILE_POSITIONS),
        COLOR_DARK_GREEN, IMAGE.MAP_EMERALD_GROVE_BACKGROUND, IMAGE.MAP_EMERALD_GROVE_TILE, COLOR_YELLOW, COLOR_WHITE, COLOR_DARK_GREEN
    );
    private static List<(int x, int y)> EMERALD_GROVE_TILE_POSITIONS =>
    [
        (2, 0), (4, 0), (8, 0), (13, 0),
        (4, 1),
        (4, 2), (6, 2), (11, 2),
        (0, 4), (1, 4), (2, 4), (7, 4), (8, 4), (9, 4),
        (5, 6)
    ];

    // Amethyst Dawn:
    private static readonly Map MAP_AMETHYST_DAWN = new(
        "Amethyst Dawn", Tile.CreateTiles(AMETHYST_DAWN_TILE_POSITIONS),
        COLOR_DARK_BLUE, IMAGE.MAP_AMETHYST_DAWN_BACKGROUND, IMAGE.MAP_AMETHYST_DAWN_TILE, COLOR_WHITE, COLOR_WHITE, COLOR_DARK_BLUE
    );
    private static List<(int x, int y)> AMETHYST_DAWN_TILE_POSITIONS =>
    [
        (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (8, 0), (9, 0), (10, 0), (11, 0), (15, 0),
        (3, 1), (9, 1),
        (13, 2),
        (0, 3), (1, 3), (12, 3),
        (4, 4), (12, 4),
        (5, 5), (6, 5), (7, 5), (10, 5), (11, 5)
    ];
    
    // All maps static list ---------------------------------------------------------------------------------------------------------------
    public static readonly List<Map> ALL_MAPS = [MAP_JUMPERS_HOME, MAP_MAGIC_TEMPLE, MAP_EMERALD_GROVE, MAP_AMETHYST_DAWN];
}
