class JSWindow {
    static #ResizeListeners = {}

    static #WidthPrevious = 0
    static #HeightPrevious = 0

    static #GetID(objRef, method) {
        return `${objRef.id}-${method}`
    }

    static #CreateListener(objRef, method) {
        return { objRef, method }
    }

    static #InvokeResizeListeners() {
        const innerWidth = window.innerWidth
        const innerHeight = window.innerHeight

        Object.entries(JSWindow.#ResizeListeners).forEach(async ([id, listener]) => {
            await listener.objRef.invokeMethodAsync(listener.method, {
                WidthPrevious: JSWindow.#WidthPrevious,
                Width: innerWidth,
                HeightPrevious: JSWindow.#HeightPrevious,
                Height: innerHeight
            })
        })

        JSWindow.#WidthPrevious = innerWidth
        JSWindow.#HeightPrevious = innerHeight
    }

    static AddResizeEventListener(objRef, method) {
        this.#WidthPrevious = window.innerWidth
        this.#HeightPrevious = window.innerHeight
        
        const listener = this.#CreateListener(objRef, method)
        this.#ResizeListeners[this.#GetID(objRef, method)] = listener

        if (Object.keys(this.#ResizeListeners).length > 1) return 
        window.addEventListener("resize", this.#InvokeResizeListeners)
    }

    static RemoveResizeEventListener(objRef, method) {
        delete this.#ResizeListeners[this.#GetID(objRef, method)]
        if (Object.keys(this.#ResizeListeners).length > 0) return 
        window.removeEventListener("resize", this.#InvokeResizeListeners)
    }

    static GetSize() {
        return { Width: window.innerWidth, Height: window.innerHeight }
    }
}

window.JSWindow = JSWindow
