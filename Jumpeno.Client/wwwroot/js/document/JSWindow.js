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

    // Touch actions ----------------------------------------------------------------------------------------------------------------------
    static #TOUCH_ACTION_AUTO = 'touch-action-auto'
    static TouchActionAutoOn() {
        document.body.classList.remove(this.#TOUCH_ACTION_AUTO)
        document.body.classList.add(this.#TOUCH_ACTION_AUTO)
    }
    static TouchActionAutoOff = () => document.body.classList.remove(this.#TOUCH_ACTION_AUTO)

    static #TOUCH_ACTION_NONE = 'touch-action-none'
    static TouchActionNoneOn() {
        document.body.classList.remove(this.#TOUCH_ACTION_NONE)
        document.body.classList.add(this.#TOUCH_ACTION_NONE)
    }
    static TouchActionNoneOff = () => document.body.classList.remove(this.#TOUCH_ACTION_NONE)

    static #TOUCH_ACTION_PAN = 'touch-action-pan'
    static TouchActionPanOn() {
        document.body.classList.remove(this.#TOUCH_ACTION_PAN)
        document.body.classList.add(this.#TOUCH_ACTION_PAN)
    }
    static TouchActionPanOff = () => document.body.classList.remove(this.#TOUCH_ACTION_PAN)

    static #TOUCH_ACTION_PINCH_ZOOM = 'touch-action-pinch-zoom'
    static TouchActionPinchZoomOn() {
        document.body.classList.remove(this.#TOUCH_ACTION_PINCH_ZOOM)
        document.body.classList.add(this.#TOUCH_ACTION_PINCH_ZOOM)
    }
    static TouchActionPinchZoomOff = () => document.body.classList.remove(this.#TOUCH_ACTION_PINCH_ZOOM)

    static #TOUCH_ACTION_MANIPULATION = 'touch-action-manipulation'
    static TouchActionManipulationOn() {
        document.body.classList.remove(this.#TOUCH_ACTION_MANIPULATION)
        document.body.classList.add(this.#TOUCH_ACTION_MANIPULATION)
    }
    static TouchActionManipulationOff = () => document.body.classList.remove(this.#TOUCH_ACTION_MANIPULATION)

    // Overscroll -------------------------------------------------------------------------------------------------------------------------
    static #OVERSCROLL_AUTO = 'overscroll-auto'
    static OverscrollAutoOn() {
        document.body.classList.remove(this.#OVERSCROLL_AUTO)
        document.body.classList.add(this.#OVERSCROLL_AUTO)
    }
    static OverscrollAutoOff = () => document.body.classList.remove(this.#OVERSCROLL_AUTO)

    static #OVERSCROLL_CONTAIN = 'overscroll-contain'
    static OverscrollContainOn() {
        document.body.classList.remove(this.#OVERSCROLL_CONTAIN)
        document.body.classList.add(this.#OVERSCROLL_CONTAIN)
    }
    static OverscrollContainOff = () => document.body.classList.remove(this.#OVERSCROLL_CONTAIN)

    static #OVERSCROLL_NONE = 'overscroll-none'
    static OverscrollNoneOn() {
        document.body.classList.remove(this.#OVERSCROLL_NONE)
        document.body.classList.add(this.#OVERSCROLL_NONE)
    }
    static OverscrollNoneOff = () => document.body.classList.remove(this.#OVERSCROLL_NONE)

    // Prevents ---------------------------------------------------------------------------------------------------------------------------
    static #Prevent = e => { if (e.cancelable) e.preventDefault() }
    
    static PreventTouchStart = () => document.body.addEventListener('touchstart', this.#Prevent)
    static DefaultTouchStart = () => document.body.removeEventListener('touchstart', this.#Prevent)
    
    static PreventTouchMove = () => document.body.addEventListener('touchmove', this.#Prevent)
    static DefaultTouchMove = () => document.body.removeEventListener('touchmove', this.#Prevent)
    
    static PreventTouchEnd = () => document.body.addEventListener('touchend', this.#Prevent)
    static DefaultTouchEnd = () => document.body.removeEventListener('touchend', this.#Prevent)
}

window.JSWindow = JSWindow
