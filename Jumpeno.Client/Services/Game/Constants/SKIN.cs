namespace Jumpeno.Client.Constants;

public enum Skin {
    MAGE_AIR,
    MAGE_EARTH,
    MAGE_FIRE,
    MAGE_ICE,
    MAGE_LAVA,
    MAGE_MAGIC,
    MAGE_PLANT,
    MAGE_SNOW,
    MAGE_WATER,
    MAGE_WOOD
}

public static class Skin_Extension
{
    public static string ToImagePath(this Skin skin) => skin switch
    {
        Skin.MAGE_AIR => ImageType.SPRITE_MAGE_AIR,
        Skin.MAGE_EARTH => ImageType.SPRITE_MAGE_EARTH,
        Skin.MAGE_FIRE => ImageType.SPRITE_MAGE_FIRE,
        Skin.MAGE_ICE => ImageType.SPRITE_MAGE_ICE,
        Skin.MAGE_LAVA => ImageType.SPRITE_MAGE_LAVA,
        Skin.MAGE_MAGIC => ImageType.SPRITE_MAGE_MAGIC,
        Skin.MAGE_PLANT => ImageType.SPRITE_MAGE_PLANT,
        Skin.MAGE_SNOW => ImageType.SPRITE_MAGE_SNOW,
        Skin.MAGE_WATER => ImageType.SPRITE_MAGE_WATER,
        Skin.MAGE_WOOD => ImageType.SPRITE_MAGE_WOOD,
        _ => throw new ArgumentOutOfRangeException(nameof(skin), skin, null)
    };
}
