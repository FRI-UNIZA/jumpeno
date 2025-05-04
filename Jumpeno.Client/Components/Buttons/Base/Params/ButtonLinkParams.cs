namespace Jumpeno.Client.Models;

public class ButtonLinkParams(
    string? Href = null,
    bool HrefPrevent = false,
    string? Label = null,
    OneOf<LINK_TARGET, string>? Target = null,
    LINK_MATCH? Match = null,
    string? ActiveClass = null
) {
    public string? Href { get; private set; } = Href;
    public bool HrefPrevent { get; private set; } = HrefPrevent;
    public string Label { get; } = Label is null ? "" : Label;
    public OneOf<LINK_TARGET, string> Target { get; set; } = Target is null ? LINK_TARGET.SELF : (OneOf<LINK_TARGET, string>) Target;
    public LINK_MATCH Match { get; set; } = Match is null ? LINK_MATCH.PREFIX : (LINK_MATCH) Match;
    public string ActiveClass { get; set; } = ActiveClass is null ? Link.DEFAULT_ACTIVE_CLASS : ActiveClass;
}
