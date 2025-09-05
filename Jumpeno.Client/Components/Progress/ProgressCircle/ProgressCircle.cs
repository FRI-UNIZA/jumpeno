namespace Jumpeno.Client.Components;

public partial class ProgressCircle {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "progress-circle";
    public const string CLASS_BACKGROUND = "progress-circle-background";
    public const string CLASS_PROGRESS = "progress-circle-progress";

    // Paramters --------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public double? Progress { get; set; } = null;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);

    protected CSSStyle ComputeStyle() {
        return new CSSStyle(Style)
        .Set("--progress", $"{Progress}", Progress is not null);
    }
}
