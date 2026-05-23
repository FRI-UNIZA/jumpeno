namespace Jumpeno.Server.Hubs;

public class ChatHub : Hub {
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(WebApplication app) => app.MapHub<ChatHub>(ChatHubConstants.URL);

    // Constants --------------------------------------------------------------------------------------------------------------------------
    private const int HistoryMessageCapacity = 10;
    private const int MinMessageLength = 2;
    private const int RateLimitMessageCount = 3;
    private const int RateLimitMessageWindowSec = 5;
    private const int DuplicateMessageLimitCount = 2;
    private const int DuplicateMessageWindowSec = 10;
    private const int MaxMessageLength = 500;

    // History ----------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<Guid, ChatMessageReceiveUpdate> MessageIndex = new();
    private static readonly Queue<Guid> MessageOrder = new();
    private static readonly object HistoryLock = new();

    // Rate limiting ----------------------------------------------------------------------------------------------------------------------
    private static readonly ConcurrentDictionary<string, Queue<DateTime>> RateWindows = new();

    // Duplicate message prevention -------------------------------------------------------------------------------------------------------
    private static readonly ConcurrentDictionary<string, Queue<(string Text, DateTime SentAt)>> DuplicateWindows = new();

    // Exceptions -------------------------------------------------------------------------------------------------------------------------
    private static async Task HandleException(IClientProxy proxy, Exception e) {
        await proxy.SendAsync(ChatHubConstants.Error, (e is AppException exception ? exception : Exceptions.Default).DTO);
        // NOTE: Client must close the connection!
    }

    private async Task HandleCallException(Exception e) {
        await HandleException(Clients.Caller, e);
        Context.Abort();
    }

    // Connect ----------------------------------------------------------------------------------------------------------------------------
    public override async Task OnConnectedAsync() {
        try {
            await AuthenticateAndBaseConnectAsync();

            InitializeConnectionTracking();

            var lastKnownId = ParseLastKnownId();

            var history = GetHistorySnapshot();
            await SendHistoryToCallerAsync(history, lastKnownId);
        } catch (Exception e) {
            await HandleCallException(e);
        }
    }

    private async Task AuthenticateAndBaseConnectAsync() {
        var ctx = Context.GetHttpContext() ?? throw Exceptions.Server;
        VersionMiddleware.CheckHubVersion(ctx);
        var token = ctx.Request.Query[ChatHubConstants.ParamAccessToken].ToString();
        JWT.Authorize(token, [Role.User, Role.Admin]);
        await UserEntity.SelectCurrentActivatedUser(); // throws if not activated
        await base.OnConnectedAsync();
    }

    private void InitializeConnectionTracking() {
        RateWindows[Context.ConnectionId] = new Queue<DateTime>();
        DuplicateWindows[Context.ConnectionId] = new Queue<(string, DateTime)>();
    }

    private Guid? ParseLastKnownId() {
        var ctx = Context.GetHttpContext() ?? throw Exceptions.Server;
        var rawLastId = ctx.Request.Query[ChatHubConstants.ParamLastMessageId].ToString();
        return Guid.TryParse(rawLastId, out var parsed) ? parsed : null;
    }

    private List<ChatMessageReceiveUpdate> GetHistorySnapshot() {
        lock (HistoryLock) {
            return MessageOrder.Select(id => MessageIndex[id]).ToList();
        }
    }

    private async Task SendHistoryToCallerAsync(List<ChatMessageReceiveUpdate> history, Guid? lastKnownId) {
        bool send = lastKnownId == null;
        foreach (var msg in history) {
            if (!send) {
                if (msg.ID == lastKnownId) send = true;  // start sending AFTER this one
                continue;
            }
            await Clients.Caller.SendAsync(ChatHubConstants.ReceiveGlobalMessage, msg);
        }
    }

    // Disconnect -------------------------------------------------------------------------------------------------------------------------
    public override async Task OnDisconnectedAsync(Exception? exception) {
        try { await base.OnDisconnectedAsync(exception); }
        catch (Exception e) { Console.Error.WriteLine(e); }
    }

    // Sanitization -----------------------------------------------------------------------------------------------------------------------
    private static string Sanitize(string text) {
        // Strip control characters (keep newlines \n and \r):
        text = new string(text.Where(c => !char.IsControl(c) || c == '\n' || c == '\r').ToArray());
        // Strip zero-width and invisible unicode:
        text = new string(text.Where(c => !char.GetUnicodeCategory(c).Equals(System.Globalization.UnicodeCategory.Format)).ToArray());
        // Collapse multiple spaces into one:
        text = System.Text.RegularExpressions.Regex.Replace(text, @" {2,}", " ");
        // Collapse more than 2 consecutive newlines into 2:
        text = System.Text.RegularExpressions.Regex.Replace(text, @"(\r?\n){3,}", "\n\n");
        return text.Trim();
    }

    // Validation -------------------------------------------------------------------------------------------------------------------------
    private void CheckRateLimit() {
        var now = DateTime.UtcNow;
        var window = RateWindows.GetOrAdd(Context.ConnectionId, _ => new Queue<DateTime>());
        lock (window) {
            // Remove timestamps outside the window:
            while (window.Count > 0 && (now - window.Peek()).TotalSeconds > RateLimitMessageWindowSec)
                window.Dequeue();
            if (window.Count >= RateLimitMessageCount)
                throw Exceptions.Values.SetErrors(Errors.Invalid.SetID("RateLimit"));
            window.Enqueue(now);
        }
    }

    private void CheckDuplicate(string text) {
        var now = DateTime.UtcNow;
        var window = DuplicateWindows.GetOrAdd(Context.ConnectionId, _ => new Queue<(string, DateTime)>());
        lock (window) {
            // Remove entries outside the window:
            while (window.Count > 0 && (now - window.Peek().SentAt).TotalSeconds > DuplicateMessageWindowSec)
                window.Dequeue();
            var recentCount = window.Count(e => e.Text == text);
            if (recentCount >= DuplicateMessageLimitCount)
                throw Exceptions.Values.SetErrors(Errors.Invalid.SetID("DuplicateMessage"));
            window.Enqueue((text, now));
        }
    }

    // Client → Server --------------------------------------------------------------------------------------------------------------------
    public async Task SendGlobalMessage(ChatMessageSendUpdate update) {
        try {
            var user = await AuthenticateAndGetUserAsync();

            var text = ValidateSanitizeAndCheck(update.Text);

            var msg = CreateAndStoreMessage(user, text);

            await BroadcastMessageAsync(msg);
        } catch (Exception e) {
            await HandleException(Clients.Caller, e);
        }
    }

    private async Task<User> AuthenticateAndGetUserAsync() {
        var ctx = Context.GetHttpContext() ?? throw Exceptions.Server;
        var token = ctx.Request.Query[ChatHubConstants.ParamAccessToken].ToString();
        JWT.Authorize(token, [Role.User, Role.Admin]);
        return await UserEntity.SelectCurrentActivatedUser();
    }

    private string ValidateSanitizeAndCheck(string? input) {
        var text = Sanitize(input ?? string.Empty);
        if (text.Length < MinMessageLength)
            throw Exceptions.Values.SetErrors(Errors.Empty.SetID(nameof(input)));
        if (text.Length > MaxMessageLength)
            throw Exceptions.Values.SetErrors(Errors.Invalid.SetID(nameof(input)));
        CheckRateLimit();
        CheckDuplicate(text);
        return text;
    }

    private ChatMessageReceiveUpdate CreateAndStoreMessage(User user, string text) {
        var msg = new ChatMessageReceiveUpdate(
            ID: Guid.NewGuid(),
            SenderName: user.Name,
            Text: text,
            SentAt: DateTime.UtcNow
        );

        lock (HistoryLock) {
            MessageIndex[msg.ID] = msg;
            MessageOrder.Enqueue(msg.ID);
            if (MessageOrder.Count > HistoryMessageCapacity) {
                var oldID = MessageOrder.Dequeue();
                MessageIndex.Remove(oldID);
            }
        }

        return msg;
    }

    private async Task BroadcastMessageAsync(ChatMessageReceiveUpdate msg) {
        await Clients.All.SendAsync(ChatHubConstants.ReceiveGlobalMessage, msg);
    }
}
