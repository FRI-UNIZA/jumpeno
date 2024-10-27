namespace Jumpeno.Client.Components;

public partial class Box {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "box";
    
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required RenderFragment ChildContent { get; set; }
    [Parameter]
    public string? Class { get; set; }
    [Parameter]
    public string? Style { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputeClass() {
        var c = new CSSClass(CLASS);
        if (Class is not null) c.Set(Class);
        return c;
    }
}
