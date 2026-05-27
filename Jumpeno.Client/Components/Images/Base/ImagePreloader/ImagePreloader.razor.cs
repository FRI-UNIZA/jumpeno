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
        Add(ImageType.BgYellow);
        Add(ImageType.BgBlue);
        Add(ImageType.JumperYellow);
        Add(ImageType.MapJumpersHomeTile);
        Add(ImageType.MapMagicTempleTile);
        Add(ImageType.MapMagicTempleBackground);
        Add(ImageType.MapEmeraldGroveTile);
        Add(ImageType.MapEmeraldGroveBackground);
        Add(ImageType.MapAmethystDawnTile);
        Add(ImageType.MapAmethystDawnBackground);
        Add(ImageType.SpriteMageAir);
        Add(ImageType.SpriteMageEarth);
        Add(ImageType.SpriteMageFire);
        Add(ImageType.SpriteMageIce);
        Add(ImageType.SpriteMageLava);
        Add(ImageType.SpriteMageMagic);
        Add(ImageType.SpriteMagePlant);
        Add(ImageType.SpriteMageSnow);
        Add(ImageType.SpriteMageWater);
        Add(ImageType.SpriteMageWood);
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
