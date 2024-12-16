class JSWindow {
    // Size -------------------------------------------------------------------------------------------------------------------------------
    static GetSize() {
        return { Width: window.innerWidth, Height: window.innerHeight }
    }

    // Resize -----------------------------------------------------------------------------------------------------------------------------
    static #WidthPrevious = 0
    static #HeightPrevious = 0

    static #WidthNow
    static #HeightNow

    static #ResizeStorage = new ListenerStorage('resize', () => ({
        WidthPrevious: JSWindow.#WidthPrevious,
        Width: JSWindow.#WidthNow,
        HeightPrevious: JSWindow.#HeightPrevious,
        Height: JSWindow.#HeightNow
    }),
    () => {
        this.#WidthNow = window.innerWidth
        this.#HeightNow = window.innerHeight
    },
    () => {
        this.#WidthPrevious = this.#WidthNow
        this.#HeightPrevious = this.#HeightNow
    })
    
    static async AddResizeEventListener(objRef, method) {
        await this.#ResizeStorage.AddEventListener(objRef, method)
    }

    static async RemoveResizeEventListener(objRef, method) {
        await this.#ResizeStorage.RemoveEventListener(objRef, method)
    }

    // KeyDown ----------------------------------------------------------------------------------------------------------------------------
    static #KeyDownStorage = new ListenerStorage('keydown', e => e.key)
    
    static async AddKeyDownEventListener(objRef, method) {
        await this.#KeyDownStorage.AddEventListener(objRef, method)
    }

    static async RemoveKeyDownEventListener(objRef, method) {
        await this.#KeyDownStorage.RemoveEventListener(objRef, method)
    }
    
    // KeyUp ------------------------------------------------------------------------------------------------------------------------------
    static #KeyUpStorage = new ListenerStorage('keyup', e => e.key)
    
    static async AddKeyUpEventListener(objRef, method) {
        await this.#KeyUpStorage.AddEventListener(objRef, method)
    }

    static async RemoveKeyUpEventListener(objRef, method) {
        await this.#KeyUpStorage.RemoveEventListener(objRef, method)
    }

    // MouseDown --------------------------------------------------------------------------------------------------------------------------
    static #MouseDownStorage = new ListenerStorage('mousedown', e => ({ X: e.clientX, Y: e.clientY }))
    
    static async AddMouseDownEventListener(objRef, method) {
        await this.#MouseDownStorage.AddEventListener(objRef, method)
    }

    static async RemoveMouseDownEventListener(objRef, method) {
        await this.#MouseDownStorage.RemoveEventListener(objRef, method)
    }

    // MouseUp ----------------------------------------------------------------------------------------------------------------------------
    static #MouseUpStorage = new ListenerStorage('mouseup', e => ({ X: e.clientX, Y: e.clientY }))
    
    static async AddMouseUpEventListener(objRef, method) {
        await this.#MouseUpStorage.AddEventListener(objRef, method)
    }

    static async RemoveMouseUpEventListener(objRef, method) {
        await this.#MouseUpStorage.RemoveEventListener(objRef, method)
    }

    // User select ------------------------------------------------------------------------------------------------------------------------
    static #NO_USER_SELECT_CLASS = 'no-user-select'
    
    static BlockUserSelect() {
        document.body.classList.remove(this.#NO_USER_SELECT_CLASS)
        document.body.classList.add(this.#NO_USER_SELECT_CLASS)
    }

    static AllowUserSelect() {
        document.body.classList.remove(this.#NO_USER_SELECT_CLASS)
    }
}

window.JSWindow = JSWindow
