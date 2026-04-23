namespace Jumpeno.Client.Models;

public class ButtonLinkParams(
    string? href = null,
    bool hrefPrevent = false,
    string? label = null,
    OneOf<WebLinkTarget, string>? target = null,
    WebLinkMatch? match = null,
    string? activeClass = null
) {
    public string? Href { get; private set; } = href;
    public bool HrefPrevent { get; private set; } = hrefPrevent;
    public string Label { get; } = label ?? "";
    public OneOf<WebLinkTarget, string> Target { get; set; } = target ?? WebLinkTarget.Self;
    public WebLinkMatch Match { get; set; } = match ?? WebLinkMatch.Prefix;
    public string ActiveClass { get; set; } = activeClass ?? WebLink.ClassDefaultActive;
}
