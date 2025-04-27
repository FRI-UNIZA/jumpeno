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
        var theme = THEME.DEFAULT;
        var text = $"<h1>{title}</h1>";
        text += $"<p>{paragraph}</p>";
        text += $"<a href=\"{link}\" class=\"email-link\">{button}</a>";
        text += "<style>";
        text +=     $"h1 {{ font-family: {theme.FONT_PRIMARY}; font-size: 20px; margin-bottom: 16px; }}";
        text +=     $"p {{ font-family: {theme.FONT_PRIMARY}; font-size: 14px; margin: 0; }}";
        text +=     ".email-link {";
        text +=         "display: inline-flex; padding: 12px 18px; border-radius: 100px;";
        text +=         $"background-color: rgb({theme.COLOR_SECONDARY_ACCENT}); color: rgb({theme.COLOR_PRIMARY}); cursor: pointer;";
        text +=         $"box-shadow: 0 1px 2px rgba({theme.COLOR_BASE}, 0.5);";
        text +=         $"font-family: {theme.FONT_PRIMARY}; font-size: 14px; font-weight: bold; text-decoration: none; letter-spacing: 0.8px;";
        text +=         "margin-top: 16px;";
        text +=         "transition: background-color 200ms;";
        text +=     "}";
        text +=     ".email-link:hover {";
        text +=         $"background-color: rgb({theme.COLOR_SECONDARY_ACCENT_HIGHLIGHT})";
        text +=     "}";
        text += "</style>";
        return text;
    }

    // Prepared actions -------------------------------------------------------------------------------------------------------------------
    public static void SendActivation(string email, string id) {
        var q = new QueryParams(); q.Set(TOKEN_TYPE.ACTIVATION.String(), JWT.GenerateActivation(Guid.Parse(id)));
        TrySend(
            email,
            I18N.T("Jumpeno activation"), 
            LINK_CONTENT(
                I18N.T("Jumpeno activation"),
                I18N.T("Hello, here is your activation link:"),
                I18N.T("Activate"),
                URL.ToAbsolute(URL.SetQueryParams(I18N.Link<HomePage>(), q))
            )
        );
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
