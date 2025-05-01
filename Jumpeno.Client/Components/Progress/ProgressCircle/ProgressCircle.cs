
namespace Jumpeno.Client.Components;

public partial class ProgressCircle {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "progress-circle";
    public const string CLASS_BACKGROUND = "progress-circle-background";
    public const string CLASS_PROGRESS = "progress-circle-progress";

    // Paramters --------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public double? Progress { get; set; } = null;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected CSSClass ComputeClass() => ComputeClass(CLASS);
    protected CSSStyle ComputeStyle() {
        var s = new CSSStyle(Style);
        if (Progress is not null) s.Set("--progress", $"{Progress}");
        return s;
    }
}
