namespace Jumpeno.Server.Constants;

public static class EMAIL_CONTENT {
    // Link -------------------------------------------------------------------------------------------------------------------------------
    public static string LINK(string title, string paragraph, string button, string link) {
        var theme = THEME.DEFAULT;
        var text = "";
        text += $"<style>";
        text +=     $".jumpeno-btn {{";
        text +=         $"background-color: {theme.EMAIL_BUTTON_BACKGROUND};";
        text +=     $"}}";
        text +=     $".jumpeno-btn:hover {{";
        text +=         $"background-color: {theme.EMAIL_BUTTON_BACKGROUND_HIGHLIGHT};";
        text +=     $"}}";
        text += $"</style>";
        text += $"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\">";
        text +=     $"<tr>";
        text +=         $"<td align=\"center\">";
        text +=             $"<div style=\"width: 100%; max-width: 480px; padding: 60px 0; margin: 20px 0px; background: {theme.EMAIL_BACKGROUND}; border-radius: 20px; text-align: center;\">";
        text +=                 $"<div style=\"padding: 0 20px\">";
        text +=                     $"<h1 style=\"font-family: {theme.EMAIL_TITLE_FONT}; color: {theme.EMAIL_TEXT_COLOR}; font-size: 24px; margin-bottom: 16px;\">";
        text +=                         title;
        text +=                     $"</h1>";
        text +=                     $"<p style=\"font-family: {theme.EMAIL_TEXT_FONT}; color: {theme.EMAIL_TEXT_COLOR}; font-size: 16px; margin: 0;\">";
        text +=                         paragraph;
        text +=                     $"</p>";
        text +=                     $"<a ";
        text +=                         $"href=\"{link}\" target=\"{WEBLINK_TARGET.BLANK}\" ";
        text +=                         $"class=\"jumpeno-btn\"";
        text +=                         $"style=\"";
        text +=                             $"display: inline-block; padding: 12px 20px; border-radius: 100px;";
        text +=                             $"color: {theme.EMAIL_BUTTON_COLOR}; cursor: pointer;";
        text +=                             $"font-family: {theme.EMAIL_BUTTON_FONT}; font-size: 14px; font-weight: bold; text-decoration: none; letter-spacing: 1px;";
        text +=                             $"margin-top: 16px;";
        text +=                             $"box-shadow: 0px 2px 4px rgba(0, 0, 0, 0.35);";
        text +=                             $"transition: background-color {theme.TRANSITION_FAST}ms ease;";
        text +=                         $"\"";
        text +=                     $">";
        text +=                         button;
        text +=                     $"</a>";
        text +=                 $"</div>";
        text +=             $"</div>";
        text +=         $"</td>";
        text +=     $"</tr>";
        text += $"</table>";
        return text;
    }
}
