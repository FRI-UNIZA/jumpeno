namespace Jumpeno.Client.Components;

public partial class PageLoaderIndicator {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASSNAME = "page-loader-indicator";
    public const string CLASSNAME_ELEMENT = "page-loader-element";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private CSSClass ComputeClass() {
        var c = new CSSClass(CLASSNAME);
        if (Class is not null) c.Set(Class);
        return c;
    }
}
