namespace Jumpeno.Client.Components;

public partial class GameInfoBox {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "game-info-box";
    public const string ClassIcon = "info-icon";
    public const string ClassText = "info-text";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string Type { get; set; }
    [Parameter]
    public required string Theme { get; set; }
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}
