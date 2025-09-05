namespace Jumpeno.Client.Models;

public class WebLinkClickEvent(WebLink link, MouseEventArgs e) {
    public WebLink Link { get; private set; } = link;
    public MouseEventArgs Event { get; private set; } = e;
}
