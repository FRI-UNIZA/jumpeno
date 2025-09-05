namespace Jumpeno.Client.Components;

public partial class CreateBox {
    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required ConnectViewModel VM { get; set; }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void Create() => Notification.Error(I18N.T("This functionality is not implemented yet."));
}
