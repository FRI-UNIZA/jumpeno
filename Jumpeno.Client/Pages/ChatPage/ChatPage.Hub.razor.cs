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
        ChatHubConnectionStatus.Connecting => I18N.T("Connecting..."),
        ChatHubConnectionStatus.Connected => I18N.T("Connected"),
        ChatHubConnectionStatus.Reconnecting => I18N.T($"Reconnecting ({VM.ReconnectAttempts}/{GlobalChatViewModel.MaxReconnectAttempts})"),
        ChatHubConnectionStatus.Disconnected => I18N.T("Disconnected"),
        ChatHubConnectionStatus.Error => I18N.T("Server error"),
        _ => string.Empty
    };

    //TODO: split into separate methods to improve the readability of the code
    private async Task ConnectToHub() {
        try {
            VM.SetConnecting();

            var q = new QueryParams();
            q.Set(ChatHubConstants.ParamAccessToken, Token.Access.raw);
            if (VM.LastReceivedMessageId.HasValue)
                q.Set(ChatHubConstants.ParamLastMessageId, VM.LastReceivedMessageId.Value.ToString());
            var hubURL = URL.SetQueryParams(URL.ToAbsolute(ChatHubConstants.URL), q);

            HubConnection = new HubConnectionBuilder()
                .WithUrl(hubURL, options => {
                    options.Headers[HEADER.ACCEPT_LANGUAGE] = I18N.Culture;
                })
                .Build();

            HubConnection.On<ChatMessageReceiveUpdate>(ChatHubConstants.ReceiveGlobalMessage, async (msg) => {
                await InvokeAsync(async () => {
                    VM.AddMessage(msg);
                    StateHasChanged();
                    await ScrollToBottom();
                });
            });

            HubConnection.On<AppExceptionDTO>(ChatHubConstants.Error, async (error) => {
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

    //TODO: split into separate methods to improve the readability of the code
    private async Task TryAutoReconnect() {
        if (VM.ReconnectAttempts >= GlobalChatViewModel.MaxReconnectAttempts) {
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
        VM.ClearMessages();
        if (HubConnection is not null) {
            try { await HubConnection.DisposeAsync(); } catch { }
            HubConnection = null;
        }
        await InvokeAsync(StateHasChanged);
        _ = InvokeAsync(ConnectToHub);
    }

    private Func<Task> BuildSendAction() => async () => {
        if (VM.Status == ChatHubConnectionStatus.Error) return;
        var text = VM.CurrentInputText.Trim();
        if (string.IsNullOrWhiteSpace(text)) return;
        if (HubConnection is null || !IsConnected) return;
        try {
            await HubConnection.SendAsync(ChatHubConstants.SendGlobalMessage, new ChatMessageSendUpdate(Text: text));
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
