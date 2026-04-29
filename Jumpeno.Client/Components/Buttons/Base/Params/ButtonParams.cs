namespace Jumpeno.Client.Models;

public class ButtonParams(
    ButtonType? type = null,
    string? label = null
) {
    public ButtonType Type { get; } = type ?? ButtonType.Button;
    public string Label { get; } = label ?? "";  
}
