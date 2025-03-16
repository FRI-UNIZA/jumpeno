namespace Jumpeno.Client.Components;

public static class ServerPageLoader {
    public static void Hide() {
        if (AppEnvironment.IsServer) return;
        JS.InvokeVoid(JSServerPageLoader.Hide);
    }
}
