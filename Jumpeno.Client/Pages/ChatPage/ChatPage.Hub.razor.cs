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

    private async Task ConnectToHub() {
        try {
            VM.SetConnecting();

            var hubURL = BuildHubUrl();
            HubConnection = CreateHubConnection(hubURL);
            RegisterHubHandlers(HubConnection);
            RegisterClosedHandler(HubConnection);

            await HubConnection.StartAsync();
            FinishConnectedState();
        } catch (Exception e) {
            Console.Error.WriteLine($"[ChatHub] Connection failed: {e.Message}");
            await TryAutoReconnect();
        }
    }

    private string BuildHubUrl() {
        var q = new QueryParams();
        q.Set(ChatHubConstants.ParamAccessToken, Token.Access.raw);
        if (VM.LastReceivedMessageId.HasValue)
            q.Set(ChatHubConstants.ParamLastMessageId, VM.LastReceivedMessageId.Value.ToString());
        return URL.SetQueryParams(URL.ToAbsolute(ChatHubConstants.URL), q);
    }

    private HubConnection CreateHubConnection(string hubURL) {
        return new HubConnectionBuilder()
            .WithUrl(hubURL, options => {
                options.Headers[Header.AcceptLanguage] = I18N.Culture;
            })
            .Build();
    }

    private void RegisterHubHandlers(HubConnection connection) {
        connection.On<ChatMessageReceiveUpdate>(ChatHubConstants.ReceiveGlobalMessage, async msg => {
            await InvokeAsync(async () => {
                VM.AddMessage(msg);
                StateHasChanged();
                await ScrollToBottom();
            });
        });

        connection.On<AppExceptionDTO>(ChatHubConstants.Error, async error => {
            await InvokeAsync(() => {
                VM.SetValidationError(error);
                StateHasChanged();
            });
        });
    }

    private void RegisterClosedHandler(HubConnection connection) {
        connection.Closed += async _ => {
            await TryAutoReconnect();
        };
    }

    private void FinishConnectedState() {
        VM.SetConnected(IsConnected);
        VM.Send = BuildSendAction();
        _ = InvokeAsync(StateHasChanged);
    }

    private async Task TryAutoReconnect() {
        if (!CanReconnect()) {
            await SetDisconnectedAsync();
            return;
        }

        await SetReconnectingAsync();
        await Task.Delay(GetReconnectDelay());

        try {
            await DisposeCurrentConnectionAsync();
            await ConnectToHub();
        } catch {
            await TryAutoReconnect();
        }
    }

    private bool CanReconnect() {
        return VM.ReconnectAttempts < GlobalChatViewModel.MaxReconnectAttempts;
    }

    private TimeSpan GetReconnectDelay() {
        return ReconnectDelays[VM.ReconnectAttempts];
    }

    private async Task SetDisconnectedAsync() {
        await InvokeAsync(() => {
            VM.SetDisconnected();
            StateHasChanged();
        });
    }

    private async Task SetReconnectingAsync() {
        await InvokeAsync(() => {
            VM.SetReconnecting();
            StateHasChanged();
        });
    }

    private async Task DisposeCurrentConnectionAsync() {
        if (HubConnection is null) return;
        await HubConnection.DisposeAsync();
        HubConnection = null;
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
