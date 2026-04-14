using System.Text.RegularExpressions;

namespace Jumpeno.Client.Pages;

public partial class ChatPage : Page {
    public const string ROUTE_EN = "/en/chat";
    public const string ROUTE_SK = "/sk/chat";
    public static readonly ROLE[] ROLES = [ROLE.USER, ROLE.ADMIN];
    private static readonly Regex GameUrlRegex =
        new(@"https?://[^\s]+/(?:en|sk)/game/([A-Z]{4})\b", System.Text.RegularExpressions.RegexOptions.Compiled);

    private bool _hubConnected = false;
    private bool _autoScrollEnabled = true;
    private bool _scrollListenerRegistered = false;

    // ViewModel --------------------------------------------------------------------------------------------------------------------------
    private GlobalChatViewModel VM = null!;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnPageInitialized() {
        VM ??= new GlobalChatViewModel(Notify);
    }

    protected override async Task OnPageAfterRenderAsync(bool firstRender) {
        await base.OnPageAfterRenderAsync(firstRender);
        if (!firstRender) return;
        if (!AppEnvironment.IsServer && !_scrollListenerRegistered) {
            ScrollArea.AddScrollListener("chatMessages", OnScrollListener);
            _scrollListenerRegistered = true;
        }
        if (_hubConnected) return;
        _hubConnected = true;
        _ = InvokeAsync(ConnectToHub);
    }

    protected override async ValueTask OnPageDisposeAsync() {
        VM.Send = () => Task.CompletedTask;
        try {
            if (!AppEnvironment.IsServer && _scrollListenerRegistered) {
                ScrollArea.RemoveScrollListener("chatMessages", OnScrollListener);
                _scrollListenerRegistered = false;
            }
        } catch { }
        await DisconnectFromHub();
        await base.OnPageDisposeAsync();
    }

    // Scroll -----------------------------------------------------------------------------------------------------------------------------
    private void OnScrollListener(ScrollAreaPosition pos) {
        var atBottom = pos.ScrollTop + pos.ClientHeight >= pos.ScrollHeight - 8;
        _autoScrollEnabled = atBottom;
        InvokeAsync(StateHasChanged);
    }

    private void JumpToBottom() {
        try {
            if (AppEnvironment.IsServer) return;
            var pos = ScrollArea.Position("chatMessages");
            var top = Math.Max(pos.ScrollHeight - pos.ClientHeight, 0);
            ScrollArea.ScrollTo("chatMessages", 0, top);
            _autoScrollEnabled = true;
            InvokeAsync(StateHasChanged);
        } catch { }
    }

    private Task ScrollToBottom() {
        try {
            if (AppEnvironment.IsServer || !_autoScrollEnabled) return Task.CompletedTask;
            var pos = ScrollArea.Position("chatMessages");
            var top = Math.Max(pos.ScrollHeight - pos.ClientHeight, 0);
            ScrollArea.ScrollTo("chatMessages", 0, top);
        } catch { }
        return Task.CompletedTask;
    }

    // Game code parsing ----------------------------------------------------------------------------------------------------------------
    private List<(string Text, string? Url)> ParseMessageSegments(string text) {
        var result = new List<(string Text, string? Url)>();
        var matches = GameUrlRegex.Matches(text);
        int last = 0;

        foreach (System.Text.RegularExpressions.Match match in matches) {
            if (match.Index > last)
                result.Add((text[last..match.Index], null));
            result.Add((match.Groups[1].Value, match.Value));
            last = match.Index + match.Length;
        }

        if (last < text.Length)
            result.Add((text[last..], null));

        return result;
    }
}
