namespace Jumpeno.Client.Models;

public class ButtonClickEvent(ButtonElement button, MouseEventArgs e) {
    public ButtonElement Button { get; set; } = button;
    public MouseEventArgs Event { get; set; } = e;
}
