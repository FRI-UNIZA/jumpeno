class JSNavigator {
    // State ------------------------------------------------------------------------------------------------------------------------------
    static State = () => window.history.state || {}

    static SetState = (state, url) => window.history.replaceState({ ...window.history.state, ...state}, "", url)
}

window.JSNavigator = JSNavigator;
