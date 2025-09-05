namespace Jumpeno.Shared.Constants;

public static class IMAGE {
    // Backgrounds:
    public static string BG_YELLOW => URL.ImageLink("backgrounds/bg-yellow.svg");
    public static string BG_BLUE => URL.ImageLink("backgrounds/bg-blue.svg");
    // Icons:
    public static string ICON_FAVICON => URL.ImageLink("icons/favicon.png");
    public static string ICON_KEYBOARD => URL.ImageLink("icons/keyboard.svg");
    // Images:
    public static string JUMPER_YELLOW => URL.ImageLink("images/jumper-yellow.png");
    public static string JUMPENO_PLAYERS => URL.ImageLink("images/jumpeno-players.png");
    public static string KEYBOARD => URL.ImageLink("images/keyboard.png", theme: true);
    public static string TILE => URL.ImageLink("images/tile.png");
    // Sprites:
    public static string SPRITE_MAGE_AIR => URL.ImageLink("sprites/mage-air.png");
    public static string SPRITE_MAGE_EARTH => URL.ImageLink("sprites/mage-earth.png");
    public static string SPRITE_MAGE_FIRE => URL.ImageLink("sprites/mage-fire.png");
    public static string SPRITE_MAGE_MAGIC => URL.ImageLink("sprites/mage-magic.png");
    public static string SPRITE_MAGE_WATER => URL.ImageLink("sprites/mage-water.png");
}
