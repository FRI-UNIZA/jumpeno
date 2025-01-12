class JSAnimator {
    static #Animators = {}
    static #Lock = new Locker()

    static #GetID(objRef, method) {
        return `${objRef.id}-${method}`
    }

    static #CreateAnimator(objRef, method) {
        return { objRef, method }
    }

    static async #NotifyAnimators() {
        await JSAnimator.#Lock.TryExclusive(async token => {
            for (const animator of Object.values(JSAnimator.#Animators)) {
                await animator.objRef.invokeMethodAsync(animator.method)
            }
            const request = Object.keys(JSAnimator.#Animators).length > 0
            token.Unlock()
            if (request) window.requestAnimationFrame(JSAnimator.#NotifyAnimators)
        })
    }

    static async AddAnimator(objRef, method) {
        await this.#Lock.Exclusive(token => {
            const animator = this.#CreateAnimator(objRef, method)
            this.#Animators[this.#GetID(objRef, method)] = animator
            const notify = Object.keys(this.#Animators).length <= 1
            token.Unlock()
            if (notify) this.#NotifyAnimators()
        })
    }

    static async RemoveAnimator(objRef, method) {
        await this.#Lock.Exclusive(() => {
            delete this.#Animators[this.#GetID(objRef, method)]
        })
    }
}

window.JSAnimator = JSAnimator
