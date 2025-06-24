namespace Jumpeno.Client.Components;

public partial class Input<T> {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "input";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected CSSClass ComputeInputClass() {
        var c = new CSSClass(CLASS);
        c.Set(Size.String());
        return c;
    }
}
