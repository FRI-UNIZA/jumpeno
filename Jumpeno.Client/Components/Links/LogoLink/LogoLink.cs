namespace Jumpeno.Client.Components;

public partial class LogoLink {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "logo-link";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public EventCallback OnClick { get; set; }
}
