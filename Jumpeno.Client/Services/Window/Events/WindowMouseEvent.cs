namespace Jumpeno.Client.Models;

public record WindowMouseEvent(
    long RawButton,
    double X,
    double Y
) {
    public MOUSE_BUTTON Button => MOUSE_BUTTON_Extension.From(RawButton);
}
