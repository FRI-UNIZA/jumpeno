namespace Jumpeno.Client.Constants;

public enum SKIN {
    MAGE_MAGIC,
    MAGE_FIRE,
    MAGE_AIR,
    MAGE_EARTH,
    MAGE_WATER
}

public static class SKIN_Extension
{
    public static string ToImagePath(this SKIN skin) => skin switch
    {
        SKIN.MAGE_MAGIC => IMAGE.SPRITE_MAGE_MAGIC,
        SKIN.MAGE_FIRE => IMAGE.SPRITE_MAGE_FIRE,
        SKIN.MAGE_AIR => IMAGE.SPRITE_MAGE_AIR,
        SKIN.MAGE_EARTH => IMAGE.SPRITE_MAGE_EARTH,
        SKIN.MAGE_WATER => IMAGE.SPRITE_MAGE_WATER,
        _ => throw new ArgumentOutOfRangeException(nameof(skin), skin, null)
    };
}
