namespace Jumpeno.Client.Params;

public record struct ConnectViewModelParams(
    bool Create,
    Func<string?> URLCode,
    EventDelegate<GameViewModel>? OnConnect = null,
    EmptyDelegate? OnDisconnect = null,
    EmptyDelegate? Notify = null
);
