namespace Jumpeno.Client.Models;

public class WebLinkKeyEvent(WebLink link, KeyboardEventArgs e) {
    public WebLink Link { get; private set; } = link;
    public KeyboardEventArgs Event { get; private set; } = e;
}
