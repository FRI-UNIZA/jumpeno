namespace Jumpeno.Client.Components;

public partial class ServerPageLoader {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public ServerPageLoaderSurface? Surface { get; set; } = ServerPageLoaderSurface.SECONDARY;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().SetSurface(Surface);

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
