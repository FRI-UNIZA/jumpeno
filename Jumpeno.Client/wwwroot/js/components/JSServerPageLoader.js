class JSServerPageLoader {
    static ID = "server-page-loader"

    static async #OnHide(loader, listener) {
        loader.removeEventListener("animationend", listener)
        await DotNet.invokeMethodAsync(DOTNET.NAMESPACE.CLIENT, DOTNET.SERVER_LOADER.LOADER_ANIMATED)
    }

    static Activate(id) {
        const loader = document.getElementById(id)
        if (!loader) return
        const listener = async () => await this.#OnHide(loader, listener)
        loader.addEventListener("animationend", listener)
    }
}

window.JSServerPageLoader = JSServerPageLoader
