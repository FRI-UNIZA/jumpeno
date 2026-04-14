namespace Jumpeno.Client.Enums;

public enum Skin {
    MageAir,
    MageEarth,
    MageFire,
    MageIce,
    MageLava,
    MageMagic,
    MagePlant,
    MageSnow,
    MageWater,
    MageWood
}

public static class Skin_Extension
{
    public static string ToImagePath(this Skin skin) => skin switch
    {
        Skin.MageAir => ImageType.SPRITE_MAGE_AIR,
        Skin.MageEarth => ImageType.SPRITE_MAGE_EARTH,
        Skin.MageFire => ImageType.SPRITE_MAGE_FIRE,
        Skin.MageIce => ImageType.SPRITE_MAGE_ICE,
        Skin.MageLava => ImageType.SPRITE_MAGE_LAVA,
        Skin.MageMagic => ImageType.SPRITE_MAGE_MAGIC,
        Skin.MagePlant => ImageType.SPRITE_MAGE_PLANT,
        Skin.MageSnow => ImageType.SPRITE_MAGE_SNOW,
        Skin.MageWater => ImageType.SPRITE_MAGE_WATER,
        Skin.MageWood => ImageType.SPRITE_MAGE_WOOD,
        _ => throw new ArgumentOutOfRangeException(nameof(skin), skin, null)
    };
}
