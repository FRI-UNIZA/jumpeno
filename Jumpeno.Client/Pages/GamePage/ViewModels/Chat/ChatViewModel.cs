namespace Jumpeno.Client.ViewModels;

public class ChatViewModel(string code, Func<GameChat?> chat, Action notify)
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    private readonly string Code = code;

    // GameChat ---------------------------------------------------------------------------------------------------------------------------
    private GameChat? Chat => chat();
    private async Task UseChat(Func<GameChat, Task> action) { if (Chat is GameChat chat) await action(chat); }

    // ViewModels -------------------------------------------------------------------------------------------------------------------------
    private readonly Action Notify = notify;

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public async Task Init()
    {
        /* TODO: Implement chat initialization */
        var _ = this;
        await Task.CompletedTask;
    }

    public async Task Dispose()
    {
        /* TODO: Implement chat disposal */
        var _ = this;
        await Task.CompletedTask;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task Open() => await UseChat(chat => chat.Open());
    public async Task Close() => await UseChat(chat => chat.Close());
}
