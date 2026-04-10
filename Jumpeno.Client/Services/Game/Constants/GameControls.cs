namespace Jumpeno.Client.Constants;

public enum GameControls {
    [StringValue("ArrowLeft")] LEFT,
    [StringValue("ArrowRight")] RIGHT,
    [StringValue(" ")] SPACE
}

public static class GameControlsExtension {
    public static GameControls? Get(string key) {
        if (key == GameControls.LEFT.String()) return GameControls.LEFT;
        else if (key == GameControls.RIGHT.String()) return GameControls.RIGHT;
        else if (key == GameControls.SPACE.String()) return GameControls.SPACE;
        return null;
    }
}
