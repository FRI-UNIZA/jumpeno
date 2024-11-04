class JSAnimator {
    static #Animators = {}

    static #GetID(objRef, method) {
        return `${objRef.id}-${method}`
    }

    static #CreateAnimator(objRef, method) {
        return { objRef, method }
    }

    static #NotifyAnimators() {
        Object.entries(JSAnimator.#Animators).forEach(async ([id, listener]) => {
            await listener.objRef.invokeMethodAsync(listener.method)
        })
        if (Object.keys(JSAnimator.#Animators).length > 0) {
            window.requestAnimationFrame(JSAnimator.#NotifyAnimators)
        }
    }

    static AddAnimator(objRef, method) {
        const animator = this.#CreateAnimator(objRef, method)
        this.#Animators[this.#GetID(objRef, method)] = animator

        if (Object.keys(this.#Animators).length > 1) return
        this.#NotifyAnimators()
    }

    static RemoveAnimator(objRef, method) {
        delete this.#Animators[this.#GetID(objRef, method)]
    }
}

window.JSAnimator = JSAnimator
