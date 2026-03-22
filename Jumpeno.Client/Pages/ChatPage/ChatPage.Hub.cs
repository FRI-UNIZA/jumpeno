namespace Jumpeno.Client.Pages;

public partial class ChatPage {
    private HubConnection? HubConnection;
    private bool IsConnected => HubConnection?.State == HubConnectionState.Connected;
    private static readonly TimeSpan[] ReconnectDelays = [
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
    ];
    private string StatusTooltip => VM.Status switch {
        CHAT_HUB_STATUS.CONNECTING => I18N.T("Connecting..."),
        CHAT_HUB_STATUS.CONNECTED => I18N.T("Connected"),
        CHAT_HUB_STATUS.RECONNECTING => I18N.T($"Reconnecting ({VM.ReconnectAttempts}/{GlobalChatViewModel.MAX_RECONNECT_ATTEMPTS})"),
        CHAT_HUB_STATUS.DISCONNECTED => I18N.T("Disconnected"),
        CHAT_HUB_STATUS.ERROR => I18N.T("Server error"),
        _ => string.Empty
    };


    private async Task ConnectToHub() {
        try {
            VM.SetConnecting();

            var q = new QueryParams();
            q.Set(CHAT_HUB.PARAM_ACCESS_TOKEN, Token.Access.raw);
            var hubURL = URL.SetQueryParams(URL.ToAbsolute(CHAT_HUB.URL), q);

            HubConnection = new HubConnectionBuilder()
                .WithUrl(hubURL, options => {
                    options.Headers[HEADER.ACCEPT_LANGUAGE] = I18N.Culture;
                })
                .Build();

            HubConnection.On<ChatMessageReceiveUpdate>(CHAT_HUB.RECEIVE_GLOBAL_MESSAGE, async (msg) => {
                Console.WriteLine($"[ChatHub] Received message from {msg.SenderName}: {msg.Text}");
                await InvokeAsync(async () => {
                    VM.AddMessage(msg);
                    StateHasChanged();
                    await ScrollToBottom();
                });
            });

            HubConnection.On<AppExceptionDTO>(CHAT_HUB.ERROR, async (error) => {
                await InvokeAsync(() => {
                    VM.SetValidationError(error);
                    StateHasChanged();
                });
            });

            HubConnection.Closed += async _ => {
                await TryAutoReconnect();
            };

            await HubConnection.StartAsync();
            VM.SetConnected(IsConnected);
            VM.Send = BuildSendAction();
            await InvokeAsync(StateHasChanged);
        } catch (Exception e) {
            Console.Error.WriteLine($"[ChatHub] Connection failed: {e.Message}");
            await TryAutoReconnect();
        }
    }

    private async Task TryAutoReconnect() {
        if (VM.ReconnectAttempts >= GlobalChatViewModel.MAX_RECONNECT_ATTEMPTS) {
            await InvokeAsync(() => {
                VM.SetDisconnected();
                StateHasChanged();
            });
            return;
        }

        var delay = ReconnectDelays[VM.ReconnectAttempts];
        await InvokeAsync(() => {
            VM.SetReconnecting();
            StateHasChanged();
        });

        await Task.Delay(delay);

        try {
            if (HubConnection is not null) {
                await HubConnection.DisposeAsync();
                HubConnection = null;
            }
            await ConnectToHub();
        } catch {
            await TryAutoReconnect();
        }
    }

    public async Task ManualReconnect() {
        VM.ResetReconnectAttempts();
        if (HubConnection is not null) {
            try { await HubConnection.DisposeAsync(); } catch { }
            HubConnection = null;
        }
        await InvokeAsync(StateHasChanged);
        _ = InvokeAsync(ConnectToHub);
    }

    private Func<Task> BuildSendAction() => async () => {
        if (VM.Status == CHAT_HUB_STATUS.ERROR) return;
        var text = VM.CurrentInputText.Trim();
        if (string.IsNullOrWhiteSpace(text)) return;
        if (HubConnection is null || !IsConnected) return;
        try {
            await HubConnection.SendAsync(CHAT_HUB.SEND_GLOBAL_MESSAGE, new ChatMessageSendUpdate(Text: text));
        } catch (Exception e) {
            Console.Error.WriteLine($"[ChatHub] Failed to send message: {e.Message}");
            return;
        }
        await InvokeAsync(() => {
            VM.InputVM.Clear();
            VM.CurrentInputText = string.Empty;
            StateHasChanged();
        });
    };

    private async Task DisconnectFromHub() {
        if (HubConnection is null) return;
        try {
            await HubConnection.StopAsync();
            await HubConnection.DisposeAsync();
        } catch { }
        finally {
            HubConnection = null;
            await InvokeAsync(() => {
                VM.SetConnected(false);
                StateHasChanged();
            });
        }
    }
}
