namespace Jumpeno.Client.Components;

public partial class WebDocument {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ID = "app-web-document";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string DocumentTitle = "";

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    private static void SetTitle(string title) {
        var instance = Instance();
        instance.DocumentTitle = title;
        AriaPageAlert.Notify(instance.DocumentTitle);
        instance.StateHasChanged();
    }
}
