class JSNavigator {
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    static Init = () => window.addEventListener(
        'popstate',
        () => DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.NAVIGATOR.POP_STATE)
    )

    // State ------------------------------------------------------------------------------------------------------------------------------
    static State = () => window.history.state || {}

    static SetState = (state, url) => window.history.replaceState({ ...window.history.state, ...state}, "", url)
}

window.JSNavigator = JSNavigator;
