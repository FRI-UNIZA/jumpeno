namespace Jumpeno.Client.Components;

public partial class ImagePreloader {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "project-image-preloader";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly List<PreloadedImage> List = [];

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static void Add(string url) => List.Add(new(url));

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    static ImagePreloader() {
        Add(IMAGE.BG_YELLOW);
        Add(IMAGE.BG_BLUE);
        Add(IMAGE.ICON_KEYBOARD);
        Add(IMAGE.JUMPER_YELLOW);
        Add(IMAGE.TILE);
        Add(IMAGE.SPRITE_MAGE_AIR);
        Add(IMAGE.SPRITE_MAGE_EARTH);
        Add(IMAGE.SPRITE_MAGE_FIRE);
        Add(IMAGE.SPRITE_MAGE_MAGIC);
        Add(IMAGE.SPRITE_MAGE_WATER);
    }

    // Styles -----------------------------------------------------------------------------------------------------------------------------
    private static string RenderStyles() {
        var styles = "";
        foreach (var image in List) {
            styles = $"{styles}\n@media (min-device-width: {image.MinDeviceWidth}px)";
            if (image.MaxDeviceWidth is not null) styles = $"{styles} and (max-device-width: {image.MaxDeviceWidth}.98px)";
            styles = $"{styles} {{ #{ID} img[src=\"{image.URL}\"] {{ display: block !important; }} }}\n";
        }
        return styles;
    }
}
