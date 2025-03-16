class JSServerPageLoader {
    static ID = "server-page-loader"
    static CLASS_HIDDEN = "hidden"

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
}

window.JSServerPageLoader = JSServerPageLoader
