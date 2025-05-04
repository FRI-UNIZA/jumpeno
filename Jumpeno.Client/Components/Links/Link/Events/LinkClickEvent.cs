namespace Jumpeno.Client.Models;

public class LinkClickEvent(Link link, MouseEventArgs e) {
    public Link Link { get; private set; } = link;
    public MouseEventArgs Event { get; private set; } = e;
}
