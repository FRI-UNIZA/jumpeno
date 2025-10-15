namespace Jumpeno.Client.Constants;

public static class IMAGE {
    // Backgrounds:
    public static string BG_YELLOW => URL.FileLink("/images/backgrounds/bg-yellow.svg");
    public static string BG_BLUE => URL.FileLink("/images/backgrounds/bg-blue.svg");
    // Favicon:
    public static string FAVICON_ICO => URL.FileLink("/images/favicon/favicon.ico");
    public static string FAVICON_PNG => URL.FileLink("/images/favicon/favicon-96x96.png");
    public static string FAVICON_SVG => URL.FileLink("/images/favicon/favicon.svg");
    public static string FAVICON_APPLE_TOUCH_ICON => URL.FileLink("/images/favicon/apple-touch-icon.png");
    // Images:
    public static string JUMPER_YELLOW => URL.FileLink("/images/images/jumper-yellow.png");
    public static string JUMPENO_PLAYERS => URL.FileLink("/images/images/jumpeno-players.png");
    public static string KEYBOARD => URL.FileLink("/images/images/keyboard.png", theme: true);
    public static string TILE => URL.FileLink("/images/images/tile.png");
    // Sprites:
    public static string SPRITE_MAGE_AIR => URL.FileLink("/images/sprites/mage-air.png");
    public static string SPRITE_MAGE_EARTH => URL.FileLink("/images/sprites/mage-earth.png");
    public static string SPRITE_MAGE_FIRE => URL.FileLink("/images/sprites/mage-fire.png");
    public static string SPRITE_MAGE_MAGIC => URL.FileLink("/images/sprites/mage-magic.png");
    public static string SPRITE_MAGE_WATER => URL.FileLink("/images/sprites/mage-water.png");
}
