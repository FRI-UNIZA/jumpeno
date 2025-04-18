namespace Jumpeno.Server.Services;

using MimeKit;
using MailKit.Net.Smtp;

public static class Email {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static string HOST => ServerSettings.Email.Host;
    public static int PORT => ServerSettings.Email.Port;
    public static string ADDRESS => ServerSettings.Email.Address;
    public static string PASSWORD => ServerSettings.Email.Password;
    public static bool MAILCATCHER => ServerSettings.Email.Mailcatcher;

    // Prepared content -------------------------------------------------------------------------------------------------------------------
    public static string LINK_CONTENT(string title, string paragraph, string button, string link) {
        var theme = ThemeProvider.DEFAULT_THEME;
        var text = $"<h1>{title}</h1>";
        text += $"<p>{paragraph}</p>";
        text += $"<a href=\"{link}\" class=\"email-link\">{button}</a>";
        text += "<style>";
        text += $"h1 {{ font-family: {theme.FONT_PRIMARY}; font-size: 20px; margin-bottom: 16px; }}";
        text += $"p {{ font-family: {theme.FONT_PRIMARY}; font-size: 14px; margin: 0; }}";
        text += ".email-link {";
        text += "display: inline-flex; padding: 12px 16px; border-radius: 100px;";
        text += $"background-color: rgb({theme.COLOR_BASE}); color: rgb({theme.COLOR_BASE_INVERT}); cursor: pointer;";
        text += $"font-family: {theme.FONT_PRIMARY}; font-size: 14px; font-weight: bold; text-decoration: none;";
        text += "margin-top: 16px;";
        text += "transition: background-color 200ms;";
        text += "}";
        text += ".email-link:hover {";
        text += $"background-color: rgb({theme.COLOR_BASE_HIGHLIGHT})";
        text += "}";
        text += "</style>";
        return text;
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void Send(string to, string subject, string content) {
        // 1) Create message:
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(ADDRESS));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Date = DateTime.UtcNow;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = content };
        // 2) Connect to SMTP server:
        using var smtp = new SmtpClient();   
        if (MAILCATCHER) {
            smtp.Connect(HOST, PORT, false);
        } else {
            smtp.Connect(HOST, PORT, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(ADDRESS, PASSWORD);
        }
        // 3) Send message:
        smtp.Send(email);
        smtp.Disconnect(true);
    }

    public static bool TrySend(string to, string subject, string content) {
        try { Send(to, subject, content); return true; }
        catch { return false; }
    }
}
