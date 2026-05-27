namespace Jumpeno.Client.Enums;

public enum GameControls {
    [StringValue("ArrowLeft")] Left,
    [StringValue("ArrowRight")] Right,
    [StringValue(" ")] Space
}

public static class GameControlsExtension {
    public static GameControls? Get(string key) {
        if (key == GameControls.Left.String()) return GameControls.Left;
        else if (key == GameControls.Right.String()) return GameControls.Right;
        else if (key == GameControls.Space.String()) return GameControls.Space;
        return null;
    }
}
