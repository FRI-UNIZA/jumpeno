class JSPageLoader {
    static ID = "page-loader"

    static AddRefreshLoader() {
        window.addEventListener("beforeunload", () => {
            DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.PAGE_LOADER.SHOW)
        });
    }

    static RequestAnimationFrame() {
        window.requestAnimationFrame(() => {
            DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.PAGE_LOADER.AFTER_ANIMATION_FRAME)
        })
    }
}

JSPageLoader.AddRefreshLoader()

window.JSPageLoader = JSPageLoader
