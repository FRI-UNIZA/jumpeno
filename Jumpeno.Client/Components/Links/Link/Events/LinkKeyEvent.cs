namespace Jumpeno.Client.Models;

public class LinkKeyEvent(Link link, KeyboardEventArgs e) {
    public Link Link { get; private set; } = link;
    public KeyboardEventArgs Event { get; private set; } = e;
}
