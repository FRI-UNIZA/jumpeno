namespace Jumpeno.Client.Components;

public partial class CookieConsentButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string COOKIE_ID = "cookie-button";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly string ID;
    protected CSSClass ComputeClass() {
        return ComputeClass(COOKIE_ID);
    }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public CookieConsentButton() {
        ID = ComponentService.GenerateID(COOKIE_ID);
    }

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static async Task OpenModal() {
        await CookieStorage.OpenModal();
    }
}
