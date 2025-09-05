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
    public EventCallback OnClick { get; set; } = EventCallback.Empty;
    [Parameter]
    public bool StopPropagation { get; set; } = false;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    private string? ComputeRole() => Label == "" ? null : "img";
    private string? ComputeLabel() => Label == "" ? null : Label;
    public override CSSClass ComputeClass() => base.ComputeClass().Set(BUTTON_ICON.CLASS);
}
