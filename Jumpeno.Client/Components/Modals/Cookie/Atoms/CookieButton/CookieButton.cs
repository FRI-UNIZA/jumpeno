namespace Jumpeno.Client.Components;

public partial class CookieButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "cookie-button";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly string ID;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CSSClass ComputeClass() => base.ComputeClass().Set(CLASS, Base);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CookieButton() => ID = IDGenerator.Generate(nameof(CookieButton));

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static async Task OpenModal() => await CookieStorage.OpenModal();
}
