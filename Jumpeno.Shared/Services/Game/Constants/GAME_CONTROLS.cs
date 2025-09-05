namespace Jumpeno.Shared.Constants;

public enum GAME_CONTROLS {
    [StringValue("ArrowLeft")] LEFT,
    [StringValue("ArrowRight")] RIGHT,
    [StringValue(" ")] SPACE
}

public static class GameControlsExtension {
    public static GAME_CONTROLS? Get(string key) {
        if (key == GAME_CONTROLS.LEFT.String()) return GAME_CONTROLS.LEFT;
        else if (key == GAME_CONTROLS.RIGHT.String()) return GAME_CONTROLS.RIGHT;
        else if (key == GAME_CONTROLS.SPACE.String()) return GAME_CONTROLS.SPACE;
        return null;
    }
}
