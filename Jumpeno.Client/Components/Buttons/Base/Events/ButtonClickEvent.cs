namespace Jumpeno.Client.Models;

public class ButtonClickEvent(ButtonComponent button, MouseEventArgs e) {
    public ButtonComponent Button { get; set; } = button;
    public MouseEventArgs Event { get; set; } = e;
}
