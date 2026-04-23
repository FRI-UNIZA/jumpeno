namespace Jumpeno.Client.Components;

public partial class ProgressCircle {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "progress-circle";
    public const string ClassBackground = "progress-circle-background";
    public const string ClassProgress = "progress-circle-progress";

    // Paramters --------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public double? Progress { get; set; } = null;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);

    protected CSSStyle ComputeStyle() {
        return new CSSStyle(Style)
        .Set("--progress", $"{Progress}", Progress is not null);
    }
}
