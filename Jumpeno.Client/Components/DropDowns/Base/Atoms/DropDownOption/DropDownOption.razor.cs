namespace Jumpeno.Client.Components;

public partial class DropDownOption {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string ClassName = "dropdown-option";

    // Parameters -------------------------------------------------------------------------------------------------------------------------
    [CascadingParameter(Name = DropDown.CascadeRef)]
    public required DropDown Ref { get; set; }
    [Parameter]
    public required string Label { get; set; }
    [Parameter]
    public required EventCallback Action { get; set; }
    [Parameter]
    public required RenderFragment ChildContent { get; set; }
}
