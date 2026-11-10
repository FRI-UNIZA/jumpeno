namespace Jumpeno.Client.Layouts;

public partial class Footer {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "footer";
    public const string ClassDisplay = "display";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public bool Display { get; set; } = true;

    // Markup -----------------------------------------------------------------------------------------------------------------------------
    public override CssClass ComputeClass() {
        return base.ComputeClass()
        .Set(ClassName, Base)
        .Set(Surface.Secondary, Base)
        .Set(ClassDisplay, Display);
    }
}
