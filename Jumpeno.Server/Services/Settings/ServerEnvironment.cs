namespace Jumpeno.Server.Services;

public static class ServerEnvironment {
    // Rendering --------------------------------------------------------------------------------------------------------------------------
    public static Microsoft.AspNetCore.Mvc.Rendering.RenderMode RenderMode() {
        return AppSettings.Prerender
            ? Microsoft.AspNetCore.Mvc.Rendering.RenderMode.WebAssemblyPrerendered
            : Microsoft.AspNetCore.Mvc.Rendering.RenderMode.WebAssembly;
    }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static bool IsStaticPath(string path) => Path.HasExtension(path);
}
