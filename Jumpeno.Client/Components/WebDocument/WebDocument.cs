namespace Jumpeno.Client.Components;

public partial class WebDocument {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "app-web-document";
    // Cascade:
    public const string CASCADE_TITLE = $"{nameof(WebDocument)}.{nameof(CASCADE_TITLE)}";

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
