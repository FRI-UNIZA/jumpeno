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
        Add(ImageType.BG_YELLOW);
        Add(ImageType.BG_BLUE);
        Add(ImageType.JUMPER_YELLOW);
        Add(ImageType.MAP_JUMPERS_HOME_TILE);
        Add(ImageType.MAP_MAGIC_TEMPLE_TILE);
        Add(ImageType.MAP_MAGIC_TEMPLE_BACKGROUND);
        Add(ImageType.MAP_EMERALD_GROVE_TILE);
        Add(ImageType.MAP_EMERALD_GROVE_BACKGROUND);
        Add(ImageType.MAP_AMETHYST_DAWN_TILE);
        Add(ImageType.MAP_AMETHYST_DAWN_BACKGROUND);
        Add(ImageType.SPRITE_MAGE_AIR);
        Add(ImageType.SPRITE_MAGE_EARTH);
        Add(ImageType.SPRITE_MAGE_FIRE);
        Add(ImageType.SPRITE_MAGE_ICE);
        Add(ImageType.SPRITE_MAGE_LAVA);
        Add(ImageType.SPRITE_MAGE_MAGIC);
        Add(ImageType.SPRITE_MAGE_PLANT);
        Add(ImageType.SPRITE_MAGE_SNOW);
        Add(ImageType.SPRITE_MAGE_WATER);
        Add(ImageType.SPRITE_MAGE_WOOD);
    }

    protected override bool ShouldComponentRender() => false;

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
