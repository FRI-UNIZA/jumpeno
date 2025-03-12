class JSConsoleUI {
    static Write = (text) => DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.CONSOLE_UI.WRITE, text)

    static WriteLine = (text) => DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.CONSOLE_UI.WRITELINE, text)

    static Clear = () => DotNet.invokeMethod(DOTNET.NAMESPACE.CLIENT, DOTNET.CONSOLE_UI.CLEAR)
}

window.JSConsoleUI = JSConsoleUI
window.ConsoleUI = JSConsoleUI
