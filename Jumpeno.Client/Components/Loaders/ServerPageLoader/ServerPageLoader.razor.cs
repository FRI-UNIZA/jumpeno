namespace Jumpeno.Client.Components;

public partial class ServerPageLoader {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public ServerPageLoaderSurface? Surface { get; set; } = ServerPageLoaderSurface.Secondary;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().SetSurface(Surface);

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void Hide() {
        if (AppEnvironment.IsServer) return;
        JS.InvokeVoid(JSServerPageLoader.Hide);
    }

    public static void Stop() {
        if (AppEnvironment.IsServer) return;
        JS.InvokeVoid(JSServerPageLoader.Stop);
    }
}
