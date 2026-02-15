namespace Jumpeno.Client.Constants;

public enum SKIN {
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

public static class SKIN_Extension
{
    public static string ToImagePath(this SKIN skin) => skin switch
    {
        SKIN.MAGE_AIR => IMAGE.SPRITE_MAGE_AIR,
        SKIN.MAGE_EARTH => IMAGE.SPRITE_MAGE_EARTH,
        SKIN.MAGE_FIRE => IMAGE.SPRITE_MAGE_FIRE,
        SKIN.MAGE_ICE => IMAGE.SPRITE_MAGE_ICE,
        SKIN.MAGE_LAVA => IMAGE.SPRITE_MAGE_LAVA,
        SKIN.MAGE_MAGIC => IMAGE.SPRITE_MAGE_MAGIC,
        SKIN.MAGE_PLANT => IMAGE.SPRITE_MAGE_PLANT,
        SKIN.MAGE_SNOW => IMAGE.SPRITE_MAGE_SNOW,
        SKIN.MAGE_WATER => IMAGE.SPRITE_MAGE_WATER,
        SKIN.MAGE_WOOD => IMAGE.SPRITE_MAGE_WOOD,
        _ => throw new ArgumentOutOfRangeException(nameof(skin), skin, null)
    };
}
