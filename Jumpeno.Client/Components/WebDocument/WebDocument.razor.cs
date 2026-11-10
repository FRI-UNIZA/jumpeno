namespace Jumpeno.Client.Components;

public partial class WebDocument {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "app-web-document";
    // Cascade:
    public const string CascadeTitle = $"{nameof(WebDocument)}.{nameof(CascadeTitle)}";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    protected string DocumentTitle = "";

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    protected static void SetTitle(string title) {
        var instance = Instance();
        instance.DocumentTitle = title;
        AriaPageAlert.Notify(instance.DocumentTitle);
        instance.StateHasChanged();
    }
}
