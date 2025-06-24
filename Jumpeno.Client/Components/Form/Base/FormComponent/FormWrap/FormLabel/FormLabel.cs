namespace Jumpeno.Client.Components;

public partial class FormLabel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "form-label";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required string For { get; set; }
    [Parameter]
    public required OneOf<string, List<string>> Label { get; set; }
    [Parameter]
    public INPUT_SIZE Size { get; set; } = INPUT_SIZE.M;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputeClass() {
        var c = ComputeClass(CLASS);
        c.Set(Size.String());
        return c;
    }

    // Fragment utils ---------------------------------------------------------------------------------------------------------------------
    public static RenderFragment Render(OneOf<string, List<string>> label) => builder => {
        if (label.IsT0) {
            builder.AddContent(0, label.AsT0);
        } else {
            int seq = 0;
            foreach (var label in label.AsT1) {
                builder.OpenElement(seq++, "span");
                builder.AddContent(seq++, label);
                builder.CloseElement();
            }
        }
    };
    public static string Main(OneOf<string, List<string>> label) {
        if (label.IsT0) return label.AsT0;
        else return label.AsT1[0];
    }

    // Parameter utils --------------------------------------------------------------------------------------------------------------------
    public static List<string> List(List<string> label) => label;
    public static string One(string label) => label;
}
