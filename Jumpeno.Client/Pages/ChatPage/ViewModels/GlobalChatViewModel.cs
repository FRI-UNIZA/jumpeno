namespace Jumpeno.Client.ViewModels;

public class GlobalChatViewModel {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const int MaxReconnectAttempts = 3;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Action NotifyCallback;
    public string CurrentInputText { get; set; } = string.Empty;
    private CancellationTokenSource? _errorCts = null;
    private int _reconnectAttempts = 0;
    public Guid? LastReceivedMessageId { get; private set; } = null;

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public void Notify() => NotifyCallback();

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public GlobalChatViewModel(Action notify) {
        NotifyCallback = notify;
        InputVM = new TextAreaViewModel(new TextAreaViewModelParams(
            Placeholder: "Type a message...",
            AutoResize: true,
            MaxLength: 500,
            OnInput: new(e => {
                CurrentInputText = e.TextAfter;
                Notify();
            }),
            OnChange: new(e => {
                CurrentInputText = e.TextAfter;
                Notify();
            }),
            OnClear: new(e => {
                CurrentInputText = string.Empty;
                Notify();
            }),
            OnEnter: new(async e => {
                if (Send is not null) await Send();
            })
        ));
    }

    // Messages ----------------------------------------------------------
    public List<ChatMessage> Messages { get; private set; } = [];

    public void AddMessage(ChatMessage message) {
        Messages.Add(message);
    }

    public void AddMessage(ChatMessageReceiveUpdate update) {
        Messages.Add(new ChatMessage(
            SenderName: update.SenderName,
            Text: update.Text,
            SentAt: update.SentAt
        ));
        LastReceivedMessageId = update.ID;
    }

    public void ClearMessages() {
        Messages.Clear();
        LastReceivedMessageId = null;
    }

    // Status ------------------------------------------------------------
    public ChatHubConnectionStatus Status { get; private set; } = ChatHubConnectionStatus.Disconnected;
    public string? ErrorMessage { get; private set; } = null;
    public int ReconnectAttempts => _reconnectAttempts;
    public bool CanManualReconnect => Status == ChatHubConnectionStatus.Disconnected && _reconnectAttempts >= MaxReconnectAttempts;

    public void SetConnected(bool connected) {
        Status = connected ? ChatHubConnectionStatus.Connected : ChatHubConnectionStatus.Disconnected;
        if (connected) _reconnectAttempts = 0;
        Notify();
    }

    public void SetConnecting() {
        Status = ChatHubConnectionStatus.Connecting;
        Notify();
    }

    public void SetReconnecting() {
        Status = ChatHubConnectionStatus.Reconnecting;
        _reconnectAttempts++;
        Notify();
    }

    public void SetDisconnected() {
        Status = ChatHubConnectionStatus.Disconnected;
        Notify();
    }

    public void SetValidationError(AppExceptionDTO dto, int seconds = 4) {
        var error = dto.Errors.FirstOrDefault();
        var message = (error?.ID, error?.Info.Key) switch {
            ("Text", var k) when k == ERROR.EMPTY.Info.Key
                => I18N.T("Message is too short (at least 2 characters)."),
            ("Text", var k) when k == ERROR.INVALID.Info.Key
                => I18N.T("Message is too long (max 500 characters)."),
            ("RateLimit", _)
                => I18N.T("You're sending messages too fast. Please slow down."),
            ("DuplicateMessage", _)
                => I18N.T("You already sent this message recently."),
            _ => I18N.T("Something went wrong. Please try again.")
        };

        _errorCts?.Cancel();
        _errorCts = new CancellationTokenSource();
        var token = _errorCts.Token;

        Status = ChatHubConnectionStatus.Error;
        ErrorMessage = message;
        Notify();

        _ = Task.Run(async () => {
            try {
                await Task.Delay(seconds * 1000, token);
                if (!token.IsCancellationRequested) {
                    Status = ChatHubConnectionStatus.Connected;
                    ErrorMessage = null;
                    Notify();
                }
            } catch (TaskCanceledException) {}
        }, token);
    }


    public void ResetReconnectAttempts() {
        _reconnectAttempts = 0;
    }

    // Input -------------------------------------------------------------
    public TextAreaViewModel InputVM { get; private set; } = null!;

    // Send --------------------------------------------------------------
    public Func<Task> Send { get; set; } = () => Task.CompletedTask;
}
