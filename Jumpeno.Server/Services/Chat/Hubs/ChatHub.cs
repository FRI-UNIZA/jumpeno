using Jumpeno.Client.Constants;

namespace Jumpeno.Server.Hubs;

#pragma warning disable CS1998

public class ChatHub : Hub {
    // Initialization ---------------------------------------------------------------------------------------------------------------------
    public static void Init(WebApplication app) => app.MapHub<ChatHub>(CHAT_HUB.URL);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static IHubContext<ChatHub> Hub => AppEnvironment.GetService<IHubContext<ChatHub>>();

    // Constants --------------------------------------------------------------------------------------------------------------------------
    private const int MAX_HISTORY = 10;
    private const int MIN_LENGTH = 2;
    private const int RATE_LIMIT_COUNT = 3;
    private const int RATE_LIMIT_WINDOW_SEC = 5;
    private const int DUPLICATE_LIMIT_COUNT = 2;
    private const int DUPLICATE_WINDOW_SEC = 10;

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
        await proxy.SendAsync(CHAT_HUB.ERROR, (e is AppException exception ? exception : EXCEPTION.DEFAULT).DTO);
        // NOTE: Client must close the connection!
    }

    private async Task HandleCallException(Exception e) {
        await HandleException(Clients.Caller, e);
        Context.Abort();
    }

    // Connect ----------------------------------------------------------------------------------------------------------------------------
    public override async Task OnConnectedAsync() {
        try {
            // 1) Check app version:
            var ctx = Context.GetHttpContext() ?? throw EXCEPTION.SERVER;
            VersionMiddleware.CheckHubVersion(ctx);
            // 2) Authorize — only activated registered/admin users may use chat:
            var token = ctx.Request.Query[CHAT_HUB.PARAM_ACCESS_TOKEN].ToString();
            JWT.Authorize(token, [ROLE.USER, ROLE.ADMIN]);
            await UserEntity.SelectCurrentActivatedUser(); // throws if not activated
            await base.OnConnectedAsync();

            // Initialize per-connection tracking:
            RateWindows[Context.ConnectionId] = new Queue<DateTime>();
            DuplicateWindows[Context.ConnectionId] = new Queue<(string, DateTime)>();
            
            // Send history to the newly connected client:
            List<ChatMessageReceiveUpdate> history;
            lock (HistoryLock) {
                history = MessageOrder.Select(id => MessageIndex[id]).ToList();
            }
            foreach (var msg in history) {
                await Clients.Caller.SendAsync(CHAT_HUB.RECEIVE_GLOBAL_MESSAGE, msg);
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
            while (window.Count > 0 && (now - window.Peek()).TotalSeconds > RATE_LIMIT_WINDOW_SEC)
                window.Dequeue();
            if (window.Count >= RATE_LIMIT_COUNT)
                throw EXCEPTION.VALUES.SetErrors(ERROR.INVALID.SetID("RateLimit"));
            window.Enqueue(now);
        }
    }

    private void CheckDuplicate(string text) {
        var now = DateTime.UtcNow;
        var window = DuplicateWindows.GetOrAdd(Context.ConnectionId, _ => new Queue<(string, DateTime)>());
        lock (window) {
            // Remove entries outside the window:
            while (window.Count > 0 && (now - window.Peek().SentAt).TotalSeconds > DUPLICATE_WINDOW_SEC)
                window.Dequeue();
            var recentCount = window.Count(e => e.Text == text);
            if (recentCount >= DUPLICATE_LIMIT_COUNT)
                throw EXCEPTION.VALUES.SetErrors(ERROR.INVALID.SetID("DuplicateMessage"));
            window.Enqueue((text, now));
        }
    }

    // Client → Server --------------------------------------------------------------------------------------------------------------------
    public async Task SendGlobalMessage(ChatMessageSendUpdate update) {
        try {
            // 1) Re-authorize on every message:
            var ctx = Context.GetHttpContext() ?? throw EXCEPTION.SERVER;
            var token = ctx.Request.Query[CHAT_HUB.PARAM_ACCESS_TOKEN].ToString();
            JWT.Authorize(token, [ROLE.USER, ROLE.ADMIN]);
            var user = await UserEntity.SelectCurrentActivatedUser();

            // 1) Sanitize:
            var text = Sanitize(update.Text ?? string.Empty);

            // 2) Validate length:
            if (text.Length < MIN_LENGTH)
                throw EXCEPTION.VALUES.SetErrors(ERROR.EMPTY.SetID(nameof(update.Text)));
            if (text.Length > 500)
                throw EXCEPTION.VALUES.SetErrors(ERROR.INVALID.SetID(nameof(update.Text)));

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
                if (MessageOrder.Count > MAX_HISTORY) {
                    var oldID = MessageOrder.Dequeue();
                    MessageIndex.Remove(oldID);
                }
            }

            await Hub.Clients.All.SendAsync(CHAT_HUB.RECEIVE_GLOBAL_MESSAGE, msg);
        } catch (Exception e) {
            await HandleException(Clients.Caller, e);
        }
    }
}
