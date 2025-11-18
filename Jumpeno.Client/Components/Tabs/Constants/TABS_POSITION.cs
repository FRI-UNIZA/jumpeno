namespace Jumpeno.Client.Constants;

public enum TABS_POSITION
{
    LEFT,
    RIGHT,
    TOP,
    BOTTOM
}

public static class TABS_POSITION_Extension
{
    public static AntDesign.TabPosition ToAntPosition(this TABS_POSITION tabPosition) => tabPosition switch
    {
        TABS_POSITION.LEFT => AntDesign.TabPosition.Left,
        TABS_POSITION.RIGHT => AntDesign.TabPosition.Right,
        TABS_POSITION.BOTTOM => AntDesign.TabPosition.Bottom,
        TABS_POSITION.TOP => AntDesign.TabPosition.Top,
        _ => throw new NotImplementedException(),
    };
}
