class JSNavigator {
    static State() {
        return window.history.state || {};
    }

    static SetState(state, url) {
        window.history.replaceState({ ...window.history.state, ...state}, "", url);
    }
}

window.JSNavigator = JSNavigator;
