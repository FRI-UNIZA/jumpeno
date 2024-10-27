namespace Jumpeno.Client.Components;

public partial class PageLoaderProgress {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME = "page-loader-progress";
    public const string CLASSNAME_ELEMENT = "page-loader-progress-element";
    
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public string? Class { get; set; }
    [Parameter]
    public string? Style { get; set; }
    [Parameter]
    public double? Progress { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputeClass() {
        var c = new CSSClass(CLASSNAME);
        if (Class is not null) c.Set(Class);
        return c;
    }
}
