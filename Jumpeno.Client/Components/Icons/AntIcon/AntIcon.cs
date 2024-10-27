namespace Jumpeno.Client.Components;

public partial class AntIcon {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string Type { get; set; }
    [Parameter]
    public required string Theme { get; set; }
    [Parameter]
    public string Label { get; set; } = "";
    [Parameter]
    public string? Style { get; set; } = null;
    [Parameter]
    public EventCallback OnClick { get; set; } = EventCallback.Empty;
    [Parameter]
    public bool StopPropagation { get; set; } = false;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string? ComputeRole() {
        return Label == "" ? null : "img";
    }

    private string? ComputeLabel() {
        return Label == "" ? null : Label;
    }
}
