namespace Jumpeno.Client.Params;

public record struct ConnectViewModelParams(
    Func<string?> URLCode,
    Func<MainLayoutViewModel?> LayoutVM,
    EventDelegate<GameViewModel>? OnConnect = null,
    EmptyDelegate? OnDisconnect = null,
    EmptyDelegate? Notify = null
);
