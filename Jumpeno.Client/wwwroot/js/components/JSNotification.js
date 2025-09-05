class JSNotification {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    static #timeouts = {}

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    static #CloseNotification(key) {
        DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.NOTIFICATION.CLOSE, key);
    }

    static #AddTimeout(key, duration) {
        if (this.#timeouts[key]) return;
        this.#timeouts[key] = setTimeout(
            () => {
                this.#CloseNotification(key)
                this.#ClearTimeout(key);
            },
            duration
        )
    }

    static #ClearTimeout(key) {
        if (!this.#timeouts[key]) return;
        clearTimeout(this.#timeouts[key]);
        delete this.#timeouts[key];
    }

    static Open(key, duration) {
        setTimeout(() => {
            const element = document.getElementById(key);
            if (element) {
                const elements = element.querySelectorAll('*:not([aria-hidden="true"])')
                elements.forEach(element => {
                    element.setAttribute('aria-hidden', 'true')
                })

                const closeIcon = element.querySelector('.ant-notification-notice-close')
                closeIcon.removeAttribute('tabindex')
                closeIcon.setAttribute('aria-label', '')
                closeIcon.addEventListener("click", () => JSActionHandler.SetFocus(JSWebDocument.ID))

                if (!duration) return
                element.onmouseover = () => this.#ClearTimeout(key);
                element.onmouseout = () => this.#AddTimeout(key, duration);
                this.#AddTimeout(key, duration);
            }
        }, 0)
    }
}

window.JSNotification = JSNotification;
