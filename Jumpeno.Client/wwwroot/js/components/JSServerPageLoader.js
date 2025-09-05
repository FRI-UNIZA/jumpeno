class JSServerPageLoader {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    static ID = "server-page-loader"
    // Class:
    static CLASS_HIDDEN = "hidden"
    static CLASS_STOPPED = "stopped"

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    static Hide() {
        const loader = document.getElementById(this.ID)
        if (!loader) return
        const listener = async () => await this.#OnHide(loader, listener)
        loader.addEventListener("animationend", listener)
        loader.classList.remove(this.CLASS_HIDDEN)
        loader.classList.add(this.CLASS_HIDDEN)
    }

    static async #OnHide(loader, listener) {
        loader.removeEventListener("animationend", listener)
        document.body.removeChild(loader)
    }

    static Stop() {
        const loader = document.getElementById(this.ID)
        if (!loader) return
        if (loader.classList.contains(this.CLASS_STOPPED)) return
        loader.classList.add(this.CLASS_STOPPED)
    }
}

window.JSServerPageLoader = JSServerPageLoader
