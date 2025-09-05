class JSWindow {
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    static Init = () => this.#InitTabReload()

    // Size -------------------------------------------------------------------------------------------------------------------------------
    static GetSize = () => ({ Width: window.innerWidth, Height: window.innerHeight })

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

    // Click ------------------------------------------------------------------------------------------------------------------------------
    static #ClickStorage = new ListenerStorage('click', e => ({ X: e.clientX, Y: e.clientY }))
    
    static async AddClickEventListener(objRef, method) {
        await this.#ClickStorage.AddEventListener(objRef, method)
    }

    static async RemoveClickEventListener(objRef, method) {
        await this.#ClickStorage.RemoveEventListener(objRef, method)
    }

    // Scroll -----------------------------------------------------------------------------------------------------------------------------
    static #ScrollStorage = new ListenerStorage('scroll')
    
    static async AddScrollEventListener(objRef, method) {
        await this.#ScrollStorage.AddEventListener(objRef, method, true)
    }

    static async RemoveScrollEventListener(objRef, method) {
        await this.#ScrollStorage.RemoveEventListener(objRef, method, true)
    }

    // User select ------------------------------------------------------------------------------------------------------------------------    
    static BlockUserSelect() {
        document.body.classList.remove(CLASS.NO_USER_SELECT)
        document.body.classList.add(CLASS.NO_USER_SELECT)
    }

    static AllowUserSelect() {
        document.body.classList.remove(CLASS.NO_USER_SELECT)
    }

    // Touch actions ----------------------------------------------------------------------------------------------------------------------
    static TouchActionAutoOn() {
        document.body.classList.remove(CLASS.TOUCH_ACTION_AUTO)
        document.body.classList.add(CLASS.TOUCH_ACTION_AUTO)
    }
    static TouchActionAutoOff = () => document.body.classList.remove(CLASS.TOUCH_ACTION_AUTO)

    static TouchActionNoneOn() {
        document.body.classList.remove(CLASS.TOUCH_ACTION_NONE)
        document.body.classList.add(CLASS.TOUCH_ACTION_NONE)
    }
    static TouchActionNoneOff = () => document.body.classList.remove(CLASS.TOUCH_ACTION_NONE)

    static TouchActionPanOn() {
        document.body.classList.remove(CLASS.TOUCH_ACTION_PAN)
        document.body.classList.add(CLASS.TOUCH_ACTION_PAN)
    }
    static TouchActionPanOff = () => document.body.classList.remove(CLASS.TOUCH_ACTION_PAN)

    static TouchActionPinchZoomOn() {
        document.body.classList.remove(CLASS.TOUCH_ACTION_PINCH_ZOOM)
        document.body.classList.add(CLASS.TOUCH_ACTION_PINCH_ZOOM)
    }
    static TouchActionPinchZoomOff = () => document.body.classList.remove(CLASS.TOUCH_ACTION_PINCH_ZOOM)

    static TouchActionManipulationOn() {
        document.body.classList.remove(CLASS.TOUCH_ACTION_MANIPULATION)
        document.body.classList.add(CLASS.TOUCH_ACTION_MANIPULATION)
    }
    static TouchActionManipulationOff = () => document.body.classList.remove(CLASS.TOUCH_ACTION_MANIPULATION)

    // Overscroll -------------------------------------------------------------------------------------------------------------------------
    static OverscrollAutoOn() {
        document.body.classList.remove(CLASS.OVERSCROLL_AUTO)
        document.body.classList.add(CLASS.OVERSCROLL_AUTO)
    }
    static OverscrollAutoOff = () => document.body.classList.remove(CLASS.OVERSCROLL_AUTO)

    static OverscrollContainOn() {
        document.body.classList.remove(CLASS.OVERSCROLL_CONTAIN)
        document.body.classList.add(CLASS.OVERSCROLL_CONTAIN)
    }
    static OverscrollContainOff = () => document.body.classList.remove(CLASS.OVERSCROLL_CONTAIN)

    static OverscrollNoneOn() {
        document.body.classList.remove(CLASS.OVERSCROLL_NONE)
        document.body.classList.add(CLASS.OVERSCROLL_NONE)
    }
    static OverscrollNoneOff = () => document.body.classList.remove(CLASS.OVERSCROLL_NONE)

    // Prevents ---------------------------------------------------------------------------------------------------------------------------
    static #Prevent = e => { if (e.cancelable) e.preventDefault() }
    
    static PreventTouchStart = () => document.body.addEventListener('touchstart', this.#Prevent)
    static DefaultTouchStart = () => document.body.removeEventListener('touchstart', this.#Prevent)
    
    static PreventTouchMove = () => document.body.addEventListener('touchmove', this.#Prevent)
    static DefaultTouchMove = () => document.body.removeEventListener('touchmove', this.#Prevent)
    
    static PreventTouchEnd = () => document.body.addEventListener('touchend', this.#Prevent)
    static DefaultTouchEnd = () => document.body.removeEventListener('touchend', this.#Prevent)

    // Inert ------------------------------------------------------------------------------------------------------------------------------
    // NOTE: Temporary Chromium fix (Browser ignores inert attribute set in Blazor)
    static Inert = () => document.querySelectorAll('[inert]').forEach(el => el.setAttribute('inert', ''));

    // Media ------------------------------------------------------------------------------------------------------------------------------
    static IsTouchDevice = () => window.matchMedia('(pointer: coarse) and (hover: none)').matches

    // Tab reload -------------------------------------------------------------------------------------------------------------------------
    static #TAB_RELOAD_ID = "JSWindow.TabReloadChannel"
    static #TabReloadChannel = new BroadcastChannel(this.#TAB_RELOAD_ID)

    static #InitTabReload = () => this.#TabReloadChannel.onmessage = () => location.reload()

    static ReloadTabs = () => this.#TabReloadChannel.postMessage({})

    // Web locks --------------------------------------------------------------------------------------------------------------------------
    static Lock = id => navigator.locks.request(id, async () => {
        await DotNet.invokeMethodAsync(DOTNET.NAMESPACE.CLIENT, DOTNET.WINDOW.LOCK, id)
    })
}

window.JSWindow = JSWindow
