namespace Jumpeno.Shared.Constants;

#pragma warning disable CA1822

public class COMMON_IMAGE {
    public string BG_YELLOW => URL.ImageLink("backgrounds/bg-yellow.svg");
    public string BG_BLUE => URL.ImageLink("backgrounds/bg-blue.svg");
    public string JUMPER_YELLOW => URL.ImageLink("images/jumper-yellow.png");
    public string JUMPENO_PLAYERS => URL.ImageLink("images/jumpeno-players.png");
    public string KEYBOARD => URL.ImageLink("images/keyboard.png");
    public string SPRITE_MAGE_AIR => URL.ImageLink("sprites/mage-air.png");
    public string SPRITE_MAGE_EARTH => URL.ImageLink("sprites/mage-earth.png");
    public string SPRITE_MAGE_FIRE => URL.ImageLink("sprites/mage-fire.png");
    public string SPRITE_MAGE_MAGIC => URL.ImageLink("sprites/mage-magic.png");
    public string SPRITE_MAGE_WATER => URL.ImageLink("sprites/mage-water.png");
    public string SPRITE_TILE => URL.ImageLink("sprites/tile.png");
}
public class LANGUAGE_IMAGE {}

public static class IMAGE {
    public static readonly COMMON_IMAGE COMMON = new();
    public static readonly LANGUAGE_IMAGE LANGUAGE = new();
}
