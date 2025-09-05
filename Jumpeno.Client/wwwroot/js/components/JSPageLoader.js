class JSPageLoader {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    static ID = "page-loader"

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    static RequestAnimationFrame() {
        window.requestAnimationFrame(() => {
            DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.PAGE_LOADER.AFTER_ANIMATION_FRAME)
        })
    }
}

window.JSPageLoader = JSPageLoader
