
namespace Jumpeno.Client.Components;

public partial class Select {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "select";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public INPUT_SIZE Size { get; set; } = INPUT_SIZE.M;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected CSSClass ComputeClass() {
        var c = ComputeClass(CLASS);
        c.Set(Size.String());
        return c;
    }
}
