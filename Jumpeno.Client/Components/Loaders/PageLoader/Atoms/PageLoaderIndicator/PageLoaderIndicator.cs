namespace Jumpeno.Client.Components;

public partial class PageLoaderIndicator {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "page-loader-indicator";
    public const string CLASS_ELEMENT = "page-loader-element";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);
}
