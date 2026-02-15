class JSNavigator {
    // State ------------------------------------------------------------------------------------------------------------------------------
    static State = key => window.history.state?.[key] || null

    static SetState = (key, state, url) => window.history.replaceState({ ...window.history.state, [key]: state}, "", url)
}

window.JSNavigator = JSNavigator;
