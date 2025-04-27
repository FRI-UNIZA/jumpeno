namespace Jumpeno.Client.Base;

public abstract class SurfaceComponent : Component {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public SURFACE Surface { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected override CSSClass ComputeClass(string className = "") {
        var c = base.ComputeClass(className);
        SurfaceClass.Apply(c, Surface);
        return c;
    }
}
