namespace Jumpeno.Client.Services.Game.Models.Map.Constants
{
    using Jumpeno.Client.Models;
    public class MAPS
    {
        #region MapsDefinitions
        //Colors -------------------------------------------------------------------------------------------
        private static readonly RGBColor ColorWhite = new(255, 255, 255);
        private static readonly RGBColor ColorYellow = new(255, 255, 0);
        private static readonly RGBColor ColorDarkPurple = new(51, 0, 51);
        private static readonly RGBColor ColorDarkGreen = new(7, 30, 24);
        private static readonly RGBColor ColorDarkBlue = new(3, 14, 22);
        private static readonly RGBColor DefaultBacgroundColor = new(36, 30, 59);
        private static readonly RGBColor DefaultBorderColor = new(10, 10, 10);

        //Tiles positions ----------------------------------------------------------------------------------
        //Jumper's home
        private static readonly List<(int x, int y)> JumpersHomeTilesPositions =
        [
            (1, 0), (2, 0), (10, 0), (12, 0),
            (12, 1),
            (5, 2), (6, 2), (7, 2), (8, 2), (9, 2),
            (12, 7), (12, 8), (13, 0)
        ];

        //Magic Temple
        private static readonly List<(int x, int y)> MagicTempleTilesPositions =
        [
            (0, 0), (1, 0), (2, 0), (8, 0), (12, 0), (13, 0), (14, 0), (15, 0),
            (0, 1), (8, 1), (15, 1),
            (15, 2),
            (2, 3), (3, 3), (4, 3), (10, 3),
            (4, 4), (5, 4), (11, 4), (13, 4),
            (5, 5), (6, 5), (8, 5)
        ];

        //Emerald Grove
        private static readonly List<(int x, int y)> EmeraldGroveTilesPositions =
        [
            (2, 0), (4, 0), (8, 0), (13, 0),
            (4, 1),
            (4, 2), (6, 2), (11, 2),
            (0, 4), (1, 4), (2, 4), (7, 4), (8, 4), (9, 4),
            (5, 6)
        ];

        //Amethyst Dawn
        private static readonly List<(int x, int y)> AmethysDawnTilesPositions =
        [
            (2, 0), (3, 0), (4, 0), (5, 0), (6, 0), (8, 0), (9, 0), (10, 0), (11, 0), (15, 0),
            (3, 1), (9, 1),
            (13, 2),
            (0, 3), (1, 3), (12, 3),
            (4, 4), (12, 4),
            (5, 5), (6, 5), (7, 5), (10, 5), (11, 5)
        ];
        #endregion

        //Static maps -------------------------------------------------------------------------------------
        //Jumper's home
        private static readonly Map JumpersHome = new("Jumper's home", Tile.CreateTiles(JumpersHomeTilesPositions, IMAGE.TILE), 
            DefaultBacgroundColor, ColorYellow, DefaultBorderColor, IMAGE.TILE, string.Empty);

        //Magic Temple
        private static readonly Map MagicTemple = new("Magic Temple", Tile.CreateTiles(MagicTempleTilesPositions, IMAGE.TILE_MAGIC_TEMPLE_ACTIVE),
            DefaultBacgroundColor, ColorWhite, ColorDarkPurple, IMAGE.TILE_MAGIC_TEMPLE_ACTIVE, IMAGE.TILE_MAGIC_TEMPLE_BACKGROUND);

        //Emerald Grove
        private static readonly Map EmeraldGrove = new("Emerald Grove", Tile.CreateTiles(EmeraldGroveTilesPositions, IMAGE.TILE_EMERALD_GROVE_ACTIVE),
            DefaultBacgroundColor, ColorYellow, ColorDarkGreen, IMAGE.TILE_EMERALD_GROVE_ACTIVE, IMAGE.TILE_EMERALD_GROVE_BACKGROUND);

        //Amethyst Dawn
        private static readonly Map AmethystDawn = new("Amethyst Dawn", Tile.CreateTiles(AmethysDawnTilesPositions, IMAGE.TILE_AMETHYST_DAWN_ACTIVE),
            DefaultBacgroundColor, ColorWhite, ColorDarkBlue, IMAGE.TILE_AMETHYST_DAWN_ACTIVE, IMAGE.TILE_AMETHYST_DAWN_BACKGROUND);

        //All maps static list ----------------------------------------------------------------------------
        public static readonly List<Map> AllMaps = [JumpersHome, MagicTemple, EmeraldGrove, AmethystDawn];
    }
}
