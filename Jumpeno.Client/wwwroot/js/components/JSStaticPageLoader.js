class JSStaticPageLoader {
    static ID = "static-page-loader"

    static async #OnOpen(loader, listener) {
        loader.removeEventListener("animationend", listener)
        await DotNet.invokeMethodAsync(DOTNET.NAMESPACE.CLIENT, DOTNET.STATIC_LOADER.LOADER_ANIMATED)
    }

    static Activate(id) {
        const loader = document.getElementById(id)
        if (!loader) return
        const listener = async () => await this.#OnOpen(loader, listener)
        loader.addEventListener("animationend", listener)
    }
}

window.JSStaticPageLoader = JSStaticPageLoader
