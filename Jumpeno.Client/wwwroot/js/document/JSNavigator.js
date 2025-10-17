class JSNavigator {
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    static Init = () => window.addEventListener(
        'popstate',
        () => DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.NAVIGATOR.POP_STATE)
    )

    // State ------------------------------------------------------------------------------------------------------------------------------
    static State = key => window.history.state?.[key] || null

    static SetState = (key, state, url) => window.history.replaceState({ ...window.history.state, [key]: state}, "", url)
}

window.JSNavigator = JSNavigator;
