namespace Jumpeno.Client.Components;

public partial class ImagePreloader {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "project-image-preloader";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly List<PreloadedImage> List = [];

    // Constants --------------------------------------------------------------------------------------------------------------------------
    static ImagePreloader() {
        List.Add(new PreloadedImage(IMAGE.COMMON.BG_YELLOW()));
        List.Add(new PreloadedImage(IMAGE.COMMON.BG_BLUE()));
        List.Add(new PreloadedImage(IMAGE.COMMON.JUMPER_YELLOW()));
    }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
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
