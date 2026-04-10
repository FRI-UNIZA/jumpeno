namespace Jumpeno.Client.Models;

public class ButtonParams(
    ButtonType? Type = null,
    string? Label = null
) {
    public ButtonType Type { get; } = Type is null ? ButtonType.BUTTON : (ButtonType) Type;
    public string Label { get; } = Label is null ? "" : Label;
}
