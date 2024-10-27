
namespace Jumpeno.Client.Components;

public partial class Select {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "select";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public INPUT_SIZE Size { get; set; } = INPUT_SIZE.M;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public static string SizeClass(INPUT_SIZE size) {
        switch (size) {
            case INPUT_SIZE.S:
                return "size-s";
            case INPUT_SIZE.M:
                return "size-m";
            case INPUT_SIZE.L:
                return "size-l";
            default:
                return "size-m";
        }
    }

    protected CSSClass ComputeClass() {
        var c = ComputeClass(CLASS);
        c.Set(SizeClass(Size));
        return c;
    }
}
