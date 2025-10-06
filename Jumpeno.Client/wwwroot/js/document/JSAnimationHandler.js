class JSAnimationHandler {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    static CLASS_DISABLED_ANIMATION = "disabled-animation";
    static CLASS_PREVENT_DISABLED_ANIMATION = "prevent-disabled-animation";

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    static DisableAnimation(selector = "body") {
        const element = document.querySelector(selector);
        if (!element) return;
        element.classList.remove(this.CLASS_DISABLED_ANIMATION);
        element.classList.add(this.CLASS_DISABLED_ANIMATION);
    }

    static RestoreAnimation(selector = "body") {
        const element = document.querySelector(selector);
        if (!element) return;
        element.classList.remove(this.CLASS_DISABLED_ANIMATION);
    }

    // Animation end ----------------------------------------------------------------------------------------------------------------------
    static async #OnAnimationEndListener(element, objRef, method, listener) {
        element.removeEventListener("animationend", listener)
        try { await objRef.invokeMethodAsync(method) } catch {}
    }
    static CallOnAnimationEnd(selector, objRef, method) {
        const element = document.querySelector(selector)
        if (!element) return
        let listener = async () => await this.#OnAnimationEndListener(element, objRef, method, listener)
        element.addEventListener("animationend", listener)
    }

    // Transition end ---------------------------------------------------------------------------------------------------------------------
    static async #OnTransitionEndListener(element, objRef, method, listener) {
        element.removeEventListener("transitionend", listener)
        try { await objRef.invokeMethodAsync(method) } catch {}
    }
    static CallOnTransitionEnd(selector, objRef, method) {
        const element = document.querySelector(selector)
        if (!element) return
        let listener = async () => await this.#OnTransitionEndListener(element, objRef, method, listener)
        element.addEventListener("transitionend", listener)
    }

    // Frames -----------------------------------------------------------------------------------------------------------------------------
    static RenderFrames(count) {
        if (count <= 0) return
        setTimeout(() => requestAnimationFrame(() => this.RenderFrames(--count)), 0)
    }
}

window.JSAnimationHandler = JSAnimationHandler
