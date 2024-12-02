class DOTNET {
    static NAMESPACE = {
        CLIENT: 'Jumpeno.Client',
        SERVER: 'Jumpeno.Server',
        SHARED: 'Jumpeno.Shared'
    }

    static IMAGE = {
        ON_LOAD: "JS_OnLoad",
        ON_ERROR: "JS_OnError"
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
