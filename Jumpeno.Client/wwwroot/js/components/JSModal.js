class JSModal {
    static #CLASSNAME_DIALOG = "modal-dialog"
    static #CLASSNAME_BACKDROP = "modal-backdrop"

    static Init() {
        window.addEventListener("keydown", async e => {
            if (e.key === 'Escape' || e.code === 'Escape') {
                await DotNet.invokeMethodAsync(DOTNET.NAMESPACE.CLIENT, DOTNET.MODAL_PROVIDER.MODAL_ESC_PRESSED)
            }
        })
    }

    static async #OnOpen(id, dialog, listener) {
        dialog.removeEventListener("animationend", listener)
        await DotNet.invokeMethodAsync(DOTNET.NAMESPACE.CLIENT, DOTNET.MODAL_PROVIDER.MODAL_OPENED, id)
    }

    static Activate(id) {
        const modal = document.getElementById(id)
        if (!modal) return
        const dialog = modal.querySelector(`.${this.#CLASSNAME_DIALOG}`)
        if (!dialog) return
        const listener = async () => await this.#OnOpen(id, dialog, listener)
        dialog.addEventListener("animationend", listener)
    }

    static PreOpen(id) {
        window.requestAnimationFrame(async () => {
            await DotNet.invokeMethodAsync(DOTNET.NAMESPACE.CLIENT, DOTNET.MODAL_PROVIDER.MODAL_PRE_OPENED, id)
        })
    }

    static async #OnClose(id, element, listener) {
        element.removeEventListener("animationend", listener)
        await DotNet.invokeMethodAsync(DOTNET.NAMESPACE.CLIENT, DOTNET.MODAL_PROVIDER.MODAL_CLOSED, id)
    }

    static Deactivate(id, loading = false) {
        const modal = document.getElementById(id)
        if (!modal) return
        const element = modal.querySelector(loading ? `.${this.#CLASSNAME_BACKDROP}` : `.${this.#CLASSNAME_DIALOG}`)
        if (!element) return
        const listener = async () => await this.#OnClose(id, element, listener)
        element.addEventListener("animationend", listener)
    }

    static AdaptCloseFocus(modalID, closeID) {
        const activeElement = document.activeElement
        const closeButton = document.getElementById(closeID)
        if (activeElement && activeElement == closeButton) {
            const modal = document.getElementById(modalID)
            const dialog = modal.querySelector(`.${this.#CLASSNAME_DIALOG}`)
            if (!dialog) return
            dialog.focus()
        }
    }
}

window.JSModal = JSModal
