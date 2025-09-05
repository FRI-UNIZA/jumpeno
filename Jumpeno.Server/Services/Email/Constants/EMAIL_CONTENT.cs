namespace Jumpeno.Server.Constants;

public static class EMAIL_CONTENT {
    // Link -------------------------------------------------------------------------------------------------------------------------------
    public static string LINK(string title, string paragraph, string button, string link) {
        var theme = THEME.DEFAULT;
        var text = "";
        text += $"<h1 style=\"font-family: {theme.FONT_PRIMARY}; color: {theme.EMAIL_COLOR}; font-size: 20px; margin-bottom: 16px;\">";
        text +=     title;
        text += "</h1>";
        text += $"<p style=\"font-family: {theme.FONT_PRIMARY}; color: {theme.EMAIL_COLOR}; font-size: 14px; margin: 0;\">";
        text +=     paragraph;
        text += "</p>";
        text += $"<a href=\"{link}\" target=\"{WEBLINK_TARGET.BLANK}\" style=\"";
        text +=     $"display: inline-flex; padding: 12px 18px; border-radius: 100px;";
        text +=     $"background-color: {theme.EMAIL_ACCENT_COLOR}; color: {theme.EMAIL_COLOR}; cursor: pointer;";
        text +=     $"font-family: {theme.FONT_PRIMARY}; font-size: 14px; font-weight: bold; text-decoration: none; letter-spacing: 0.8px;";
        text +=     $"margin-top: 16px;";
        text += "\">";
        text +=     button;
        text += "</a>";
        return text;
    }
}
