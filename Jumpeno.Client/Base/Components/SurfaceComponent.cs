namespace Jumpeno.Client.Base;

public abstract class SurfaceComponent : Component {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter]
    public SURFACE Surface { get; set; }
    [Parameter]
    public string Class { get; set; } = "";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected CSSClass ComputeClass(string className = "") {
        var c = new CSSClass(className);
        SurfaceClass.Apply(c, Surface);
        c.Set(Class);
        return c;
    }
}
