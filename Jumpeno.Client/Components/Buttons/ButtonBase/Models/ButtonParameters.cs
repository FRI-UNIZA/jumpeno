namespace Jumpeno.Client.Models;

public class ButtonParameters(
    BUTTON_TYPE? Type = null,
    string? Label = null
) {
    public BUTTON_TYPE Type { get; } = Type is null ? BUTTON_TYPE.BUTTON : (BUTTON_TYPE) Type;
    public string Label { get; } = Label is null ? "" : Label;
}
