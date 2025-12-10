namespace Jumpeno.Client.Models;

public record struct ConnectViewModelParams(
    bool Create,
    Func<string?> URLCode,
    Func<GameChat?> Chat,
    EventDelegate<GameViewModel>? OnConnect = null,
    EmptyDelegate? OnDisconnect = null,
    Action? Notify = null
);
