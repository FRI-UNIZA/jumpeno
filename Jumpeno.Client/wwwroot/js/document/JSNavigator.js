class JSNavigator {
    // State ------------------------------------------------------------------------------------------------------------------------------
    static State = () => window.history.state || {}

    static SetState = (state, url) => window.history.replaceState({ ...window.history.state, ...state}, "", url)

    // Media ------------------------------------------------------------------------------------------------------------------------------
    static IsTouchDevice = () => window.matchMedia('(pointer: coarse) and (hover: none)').matches

}

window.JSNavigator = JSNavigator;
