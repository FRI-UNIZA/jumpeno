namespace Jumpeno.Client.ViewModels;

public class LoadAreaViewModel(string? ID = null, bool loading = false) : ViewModel<LoadArea> {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public readonly string ID = ID ?? IDGenerator.Generate(LoadArea.CLASS);
    public bool Loading { get; private set; } = loading;
    // State:
    private readonly LockerSlim Lock = new();
    // Loading:
    private readonly MinWatch MinWatch = new(LoadArea.MIN_LOADING);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public async Task OnViewDispose() => await Lock.DisposeSafe();

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public async Task StartLoading(bool preventScroll = true) {
        await Lock.TryExclusive(async () => {
            if (AppEnvironment.IsClient) MinWatch.Start();
            Loading = true;
            await NotifyAsync(MESSAGE_START, new MessageStartData(preventScroll)); 
        });
    }

    public async Task FinishLoading(bool minLoading = true, bool restoreFocus = false, bool preventScroll = true) {
        await Lock.TryExclusive(async () => {
            if (AppEnvironment.IsClient && minLoading) await MinWatch.Task;
            Loading = false;
            await NotifyAsync(MESSAGE_FINISH, new MessageFinishData(restoreFocus, preventScroll));
        });
    }

    public async Task RestoreFocus(string id, bool preventScroll = true) {
        await NotifyAsync(MESSAGE_RESTORE, new MessageRestoreData(id, preventScroll));
    }
    
    public void SetRestoreID(string id) {
        Notify(MESSAGE_SET_RESTORE_ID, new MessageSetRestoreIDData(id));
    }

    public bool HasFocus() => AppEnvironment.IsClient && ActionHandler.HasFocus($"#{ID}");

    public void Focus(bool preventScroll = true) { if (AppEnvironment.IsClient) ActionHandler.SetFocus(ID, preventScroll: preventScroll); }

    
    // Notification -----------------------------------------------------------------------------------------------------------------------
    public const string MESSAGE_START = $"{nameof(StartLoading)}"; public record MessageStartData(bool PreventScroll);
    public const string MESSAGE_FINISH = $"{nameof(FinishLoading)}"; public record MessageFinishData(bool RestoreFocus, bool PreventScroll);
    public const string MESSAGE_RESTORE = $"{nameof(RestoreFocus)}"; public record MessageRestoreData(string ID, bool PreventScroll);
    public const string MESSAGE_SET_RESTORE_ID = $"{nameof(SetRestoreID)}"; public record MessageSetRestoreIDData(string ID);
}
