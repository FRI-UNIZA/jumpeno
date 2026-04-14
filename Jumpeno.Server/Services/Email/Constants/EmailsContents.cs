namespace Jumpeno.Server.Constants;

public static class EmailsContents {
    // Link -------------------------------------------------------------------------------------------------------------------------------
    public static string LINK(string title, string paragraph, string button, string link) {
        var theme = ThemeType.DEFAULT;
        var text = "";
        text += $"<!DOCTYPE html>";
        text += $"<html lang=\"{I18N.Culture}\">";
        text +=     $"<head>";
        text +=         $"<meta charset=\"UTF-8\">";
        text +=         $"<style>";
        text +=             $".jumpeno-btn {{";
        text +=                 $"background-color: {theme.EMAIL_BUTTON_BACKGROUND} !important;";
        text +=             $"}} ";
        text +=             $".jumpeno-btn:hover {{";
        text +=                 $"background-color: {theme.EMAIL_BUTTON_BACKGROUND_HIGHLIGHT} !important;";
        text +=             $"}}";
        text +=         $"</style>";
        text +=     $"</head>";
        text +=     $"<body style=\"padding: 0; margin: 0;\">";        
        text +=         $"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\">";
        text +=             $"<tr>";
        text +=                 $"<td align=\"center\" style=\"padding: 20px 8px\">";
        text +=                     $"<div style=\"width: 100%; max-width: 480px; padding: 60px 0; margin: 0; background: {theme.EMAIL_BACKGROUND}; border-radius: 20px; text-align: center;\">";
        text +=                         $"<div style=\"padding: 0 20px\">";
        text +=                             $"<h1 style=\"font-family: {theme.EMAIL_TITLE_FONT}; color: {theme.EMAIL_TEXT_COLOR}; font-size: 24px; margin-bottom: 16px;\">";
        text +=                                 title;
        text +=                             $"</h1>";
        text +=                             $"<p style=\"font-family: {theme.EMAIL_TEXT_FONT}; color: {theme.EMAIL_TEXT_COLOR}; font-size: 16px; margin: 0;\">";
        text +=                                 paragraph;
        text +=                             $"</p>";
        text +=                             $"<a ";
        text +=                                 $"href=\"{link}\" target=\"{WebLinkTarget.Blank}\" ";
        text +=                                 $"class=\"jumpeno-btn\"";
        text +=                                 $"style=\"";
        text +=                                     $"display: inline-block; padding: 12px 20px; border-radius: 100px;";
        text +=                                     $"color: {theme.EMAIL_BUTTON_COLOR}; cursor: pointer;";
        text +=                                     $"background-color: {theme.EMAIL_BUTTON_BACKGROUND};";
        text +=                                     $"font-family: {theme.EMAIL_BUTTON_FONT}; font-size: 14px; font-weight: bold; text-decoration: none; letter-spacing: 1px;";
        text +=                                     $"margin-top: 16px;";
        text +=                                     $"border-bottom: 2px solid rgba(0, 0, 0, 0.15);";
        text +=                                     $"transition: background-color {theme.TRANSITION_FAST}ms ease;";
        text +=                                 $"\" ";
        text +=                                 $"data-ogsb data-ogsc";
        text +=                             $">";
        text +=                                 button;
        text +=                             $"</a>";
        text +=                         $"</div>";
        text +=                     $"</div>";
        text +=                 $"</td>";
        text +=             $"</tr>";
        text +=         $"</table>";
        text +=     $"</body>";
        text += $"</html>";
        return text;
    }
}
