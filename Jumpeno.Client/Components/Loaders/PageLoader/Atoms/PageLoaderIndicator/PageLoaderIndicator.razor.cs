namespace Jumpeno.Client.Components;

public partial class PageLoaderIndicator {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "page-loader-indicator";
    public const string ClassElement = "page-loader-element";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);
}
