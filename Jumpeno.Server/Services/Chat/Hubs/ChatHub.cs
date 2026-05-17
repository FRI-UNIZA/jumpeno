namespace Jumpeno.Server.Hubs;

public class ChatHub : Hub {
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(WebApplication app) => app.MapHub<ChatHub>(ChatHubConstants.URL);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static IHubContext<ChatHub> Hub => AppEnvironment.GetService<IHubContext<ChatHub>>();

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
    //TODO: XML documentation comments (/// <summary>) or extraction into better-named private methods
    public override async Task OnConnectedAsync() {
        try {
            // 1) Check app version:
            var ctx = Context.GetHttpContext() ?? throw Exceptions.Server;
            VersionMiddleware.CheckHubVersion(ctx);
            // 2) Authorize — only activated registered/admin users may use chat:
            var token = ctx.Request.Query[ChatHubConstants.ParamAccessToken].ToString();
            JWT.Authorize(token, [Role.User, Role.Admin]);
            await UserEntity.SelectCurrentActivatedUser(); // throws if not activated
            await base.OnConnectedAsync();

            // Initialize per-connection tracking:
            RateWindows[Context.ConnectionId] = new Queue<DateTime>();
            DuplicateWindows[Context.ConnectionId] = new Queue<(string, DateTime)>();

            var rawLastId = ctx.Request.Query[ChatHubConstants.ParamLastMessageId].ToString();
            Guid? lastKnownId = Guid.TryParse(rawLastId, out var parsed) ? parsed : null;

            
            // Send history to the newly connected client:
            List<ChatMessageReceiveUpdate> history;
            lock (HistoryLock) {
                history = MessageOrder.Select(id => MessageIndex[id]).ToList();
            }
            bool send = lastKnownId == null;
            foreach (var msg in history) {
                if (!send) {
                    if (msg.ID == lastKnownId) send = true;  // start sending AFTER this one
                    continue;
                }
                await Clients.Caller.SendAsync(ChatHubConstants.ReceiveGlobalMessage, msg);
            }
        } catch (Exception e) {
            await HandleCallException(e);
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
    //TODO: XML documentation comments (/// <summary>) or extraction into better-named private methods
    public async Task SendGlobalMessage(ChatMessageSendUpdate update) {
        try {
            // 1) Re-authorize on every message:
            var ctx = Context.GetHttpContext() ?? throw Exceptions.Server;
            var token = ctx.Request.Query[ChatHubConstants.ParamAccessToken].ToString();
            JWT.Authorize(token, [Role.User, Role.Admin]);
            var user = await UserEntity.SelectCurrentActivatedUser();

            // 1) Sanitize:
            var text = Sanitize(update.Text ?? string.Empty);

            // 2) Validate length:
            if (text.Length < MinMessageLength)
                throw Exceptions.Values.SetErrors(Errors.Empty.SetID(nameof(update.Text)));
            if (text.Length > MaxMessageLength)
                throw Exceptions.Values.SetErrors(Errors.Invalid.SetID(nameof(update.Text)));

            // 3) Rate limit:
            CheckRateLimit();

            // 4) Duplicate check:
            CheckDuplicate(text);

            var msg = new ChatMessageReceiveUpdate(
                ID: Guid.NewGuid(),
                SenderName: user.Name,
                Text: text,
                SentAt: DateTime.UtcNow
            );

            // Store in history:
            lock (HistoryLock) {
                MessageIndex[msg.ID] = msg;
                MessageOrder.Enqueue(msg.ID);
                if (MessageOrder.Count > HistoryMessageCapacity) {
                    var oldID = MessageOrder.Dequeue();
                    MessageIndex.Remove(oldID);
                }
            }

            await Hub.Clients.All.SendAsync(ChatHubConstants.ReceiveGlobalMessage, msg);
        } catch (Exception e) {
            await HandleException(Clients.Caller, e);
        }
    }
}
