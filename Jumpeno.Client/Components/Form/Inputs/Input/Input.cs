
namespace Jumpeno.Client.Components;

public partial class Input<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "input";

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

    protected CSSClass ComputeInputClass() {
        var c = new CSSClass(CLASS);
        c.Set(SizeClass(Size));
        return c;
    }
}
