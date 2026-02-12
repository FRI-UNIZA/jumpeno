namespace Jumpeno.Client.Services;

public partial class UI : ServiceComponent<UI> {
    // Lock -------------------------------------------------------------------------------------------------------------------------------
    private readonly LockerSlim UILock = new();
    public static LockerSlim Lock => Instance().UILock; 

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override async ValueTask OnComponentDisposeAsync() {
        await UILock.DisposeSafe();
    }
}
