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

public static class SkinExtension
{
    public static string ToImagePath(this Skin skin) => skin switch
    {
        Skin.MageAir => ImageType.SpriteMageAir,
        Skin.MageEarth => ImageType.SpriteMageEarth,
        Skin.MageFire => ImageType.SpriteMageFire,
        Skin.MageIce => ImageType.SpriteMageIce,
        Skin.MageLava => ImageType.SpriteMageLava,
        Skin.MageMagic => ImageType.SpriteMageMagic,
        Skin.MagePlant => ImageType.SpriteMagePlant,
        Skin.MageSnow => ImageType.SpriteMageSnow,
        Skin.MageWater => ImageType.SpriteMageWater,
        Skin.MageWood => ImageType.SpriteMageWood,
        _ => throw new ArgumentOutOfRangeException(nameof(skin), skin, null)
    };
}
