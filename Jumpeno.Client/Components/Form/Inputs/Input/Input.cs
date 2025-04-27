
namespace Jumpeno.Client.Components;

public partial class Input<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "input";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public INPUT_SIZE Size { get; set; } = INPUT_SIZE.M;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected CSSClass ComputeInputClass() {
        var c = new CSSClass(CLASS);
        c.Set(Size.String());
        return c;
    }
}
