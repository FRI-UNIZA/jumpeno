namespace Jumpeno.Client.Models;

public class ButtonLinkParams(
    string? Href = null,
    bool HrefPrevent = false,
    string? Label = null,
    OneOf<WEBLINK_TARGET, string>? Target = null,
    WEBLINK_MATCH? Match = null,
    string? ActiveClass = null
) {
    public string? Href { get; private set; } = Href;
    public bool HrefPrevent { get; private set; } = HrefPrevent;
    public string Label { get; } = Label is null ? "" : Label;
    public OneOf<WEBLINK_TARGET, string> Target { get; set; } = Target is null ? WEBLINK_TARGET.SELF : (OneOf<WEBLINK_TARGET, string>) Target;
    public WEBLINK_MATCH Match { get; set; } = Match is null ? WEBLINK_MATCH.PREFIX : (WEBLINK_MATCH) Match;
    public string ActiveClass { get; set; } = ActiveClass is null ? WebLink.CLASS_DEFAULT_ACTIVE : ActiveClass;
}
