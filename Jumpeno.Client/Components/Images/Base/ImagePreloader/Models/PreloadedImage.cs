namespace Jumpeno.Client.Models;

public record PreloadedImage(
    string URL,
    int MinDeviceWidth = 0,
    int? MaxDeviceWidth = null
) {}
