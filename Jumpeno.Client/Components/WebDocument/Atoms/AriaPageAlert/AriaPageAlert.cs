namespace Jumpeno.Client.Atoms;

public partial class AriaPageAlert {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private string PageTitle = "";

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static void Notify(string title) {
        if (AppEnvironment.IsServer) return;
        var instance = Instance();
        instance.PageTitle = title;
        instance.StateHasChanged();
    }
}
