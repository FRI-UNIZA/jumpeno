namespace Jumpeno.Client.Components;

public partial class DropDownOption {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string CLASS = "dropdown-option";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [Parameter]
    public required DropDown Ref { get; set; }
    [Parameter]
    public required string Label { get; set; }
    [Parameter]
    public required EmptyDelegate Action { get; set; }
    [Parameter]
    public required RenderFragment ChildContent { get; set; }
}
