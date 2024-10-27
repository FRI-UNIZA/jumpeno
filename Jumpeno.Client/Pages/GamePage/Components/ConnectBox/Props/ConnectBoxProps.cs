namespace Jumpeno.Client.Props;

public record ConnectBoxProps(
    Func<string> Key,
    Func<string>? DefaultCode = null,
    EventDelegate<ConnectData>? OnPlay = null,
    EventDelegate<ConnectData>? OnWatch = null
);
