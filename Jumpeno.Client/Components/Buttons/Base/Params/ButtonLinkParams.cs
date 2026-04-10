namespace Jumpeno.Client.Models;

public class ButtonLinkParams(
    string? Href = null,
    bool HrefPrevent = false,
    string? Label = null,
    OneOf<WebLinkTarget, string>? Target = null,
    WebLinkMatch? Match = null,
    string? ActiveClass = null
) {
    public string? Href { get; private set; } = Href;
    public bool HrefPrevent { get; private set; } = HrefPrevent;
    public string Label { get; } = Label is null ? "" : Label;
    public OneOf<WebLinkTarget, string> Target { get; set; } = Target is null ? WebLinkTarget.SELF : (OneOf<WebLinkTarget, string>) Target;
    public WebLinkMatch Match { get; set; } = Match is null ? WebLinkMatch.PREFIX : (WebLinkMatch) Match;
    public string ActiveClass { get; set; } = ActiveClass is null ? WebLink.CLASS_DEFAULT_ACTIVE : ActiveClass;
}
