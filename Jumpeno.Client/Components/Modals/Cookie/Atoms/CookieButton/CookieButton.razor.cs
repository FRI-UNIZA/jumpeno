namespace Jumpeno.Client.Components;

public partial class CookieButton {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "cookie-button";

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly string ID;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() => base.ComputeClass().Set(ClassName, Base);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    public CookieButton() => ID = IDGenerator.Generate(nameof(CookieButton));

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static async Task OpenModal() => await CookieModal.Open();
}
