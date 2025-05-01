namespace Jumpeno.Client.Utils;

public static class SurfaceClass {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string PREFIX = "surface";
    public const string ALL = "surface-all";

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static void Apply(CSSClass @class, SURFACE surface) {
        @class.Set($"{PREFIX}-{surface.StringLower()}");
        @class.Set(ALL);
    }
}
