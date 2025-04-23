class DOTNET {
    static NAMESPACE = {
        CLIENT: 'Jumpeno.Client',
        SERVER: 'Jumpeno.Server',
        SHARED: 'Jumpeno.Shared'
    }

    static WINDOW = {
        LOCK: "JS_Lock"
    }

    // NOTE: No JS prefix
    static CONSOLE_UI = {
        WRITE: "Write",
        WRITELINE: "WriteLine",
        CLEAR: "Clear"
    }

    static IMAGE = {
        ON_LOAD: "JS_OnLoad",
        ON_ERROR: "JS_OnError"
    }

    static NAVIGATOR = {
        POP_STATE: "JS_PopState"
    }

    static NOTIFICATION = {
        CLOSE: "JS_Close"
    }

    static PAGE_LOADER = {
        SHOW: "JS_Show",
        AFTER_ANIMATION_FRAME: "JS_AfterAnimationFrame"
    }

    static SERVER_LOADER = {
        LOADER_ANIMATED: "JS_LoaderAnimated"
    }

    static SCROLL_AREA = {
        ON_SCROLL: "JS_OnScroll"
    }

    static MODAL_PROVIDER = {
        MODAL_PRE_OPENED: "JS_ModalPreOpened",
        MODAL_OPENED: "JS_ModalOpened",
        MODAL_CLOSED: "JS_ModalClosed",
        MODAL_ESC_PRESSED: "JS_ModalESCPressed"
    }
}

window.DOTNET = DOTNET
