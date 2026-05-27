namespace Jumpeno.Client.Models;

public record WindowMouseEvent(
    long RawButton,
    double X,
    double Y
) {
    public MouseButton Button => MouseButtonExtension.From(RawButton);
}
