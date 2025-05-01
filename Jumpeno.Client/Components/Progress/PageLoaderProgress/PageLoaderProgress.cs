namespace Jumpeno.Client.Components;

public partial class PageLoaderProgress {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME = "page-loader-progress";
    public const string CLASSNAME_ELEMENT = "page-loader-progress-element";
    
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public double? Progress { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputeClass() => ComputeClass(CLASSNAME);
}
