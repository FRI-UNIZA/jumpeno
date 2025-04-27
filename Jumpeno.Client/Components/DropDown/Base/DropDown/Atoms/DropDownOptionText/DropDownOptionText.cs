namespace Jumpeno.Client.Components;

public partial class DropDownOptionText {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "dropdown-option-text";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required RenderFragment ChildContent { get; set; }
}
