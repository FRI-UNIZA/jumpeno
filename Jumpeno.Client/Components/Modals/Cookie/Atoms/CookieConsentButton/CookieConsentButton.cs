namespace Jumpeno.Client.Components;

public partial class CookieConsentButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string COOKIE_ID = "cookie-button";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly string ID;
    protected CSSClass ComputeClass() => ComputeClass(COOKIE_ID);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CookieConsentButton() => ID = IDGenerator.Generate(COOKIE_ID);

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static async Task OpenModal() => await CookieStorage.OpenModal();
}
