namespace Jumpeno.Client.Models;

public record WindowResizeEvent(
    double WidthPrevious,
    double Width,
    double HeightPrevious,
    double Height
);
