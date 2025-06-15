namespace Jumpeno.Server.Services;

using MimeKit;
using MailKit.Net.Smtp;

public static class Email {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static string HOST => ServerSettings.Email.Host;
    public static int PORT => ServerSettings.Email.Port;
    public static string ADDRESS => AppSettings.Email;
    public static string PASSWORD => ServerSettings.Email.Password;
    public static string BACKUP_KEYS => ServerSettings.Email.BackupKeys;
    public static string APP_PASSWORD => ServerSettings.Email.AppPassword;
    public static bool MAILCATCHER => ServerSettings.Email.Mailcatcher;

    // Prepared content -------------------------------------------------------------------------------------------------------------------
    public static string LINK_CONTENT(string title, string paragraph, string button, string link) {
        var theme = THEME.DEFAULT;
        var text = "";
        text += $"<h1 style=\"font-family: {theme.FONT_PRIMARY}; color: rgb({theme.COLOR_PRIMARY}); font-size: 20px; margin-bottom: 16px;\">";
        text +=     title;
        text += "</h1>";
        text += $"<p style=\"font-family: {theme.FONT_PRIMARY}; color: rgb({theme.COLOR_PRIMARY}); font-size: 14px; margin: 0;\">";
        text +=     paragraph;
        text += "</p>";
        text += $"<a href=\"{link}\" target=\"{LINK_TARGET.BLANK}\" style=\"";
        text +=     $"display: inline-flex; padding: 12px 18px; border-radius: 100px;";
        text +=     $"background-color: rgb({theme.COLOR_SECONDARY_ACCENT}); color: rgb({theme.COLOR_PRIMARY}); cursor: pointer;";
        text +=     $"font-family: {theme.FONT_PRIMARY}; font-size: 14px; font-weight: bold; text-decoration: none; letter-spacing: 0.8px;";
        text +=     $"margin-top: 16px;";
        text += "\">";
        text +=     button;
        text += "</a>";
        return text;
    }

    // Prepared actions -------------------------------------------------------------------------------------------------------------------
    public static void SendActivation(string email, string id) {
        try {
            var q = new QueryParams(); q.Set(TOKEN_TYPE.ACTIVATION.String(), JWT.GenerateActivation(Guid.Parse(id)));
            Send(
                email,
                I18N.T("Jumpeno activation"),
                LINK_CONTENT(
                    I18N.T("Jumpeno activation"),
                    I18N.T("Hello, here is your activation link:"),
                    I18N.T("Activate"),
                    URL.ToAbsolute(URL.SetQueryParams(I18N.Link<HomePage>(), q))
                )
            );
        } catch {
            throw EXCEPTION.SERVER.SetInfo(I18N.T("Failed to send activation email."));
        }
    }

    public static bool TrySendActivation(string email, string id) {
        try { SendActivation(email, id); return true; }
        catch { return false; }
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
            smtp.Authenticate(ADDRESS, APP_PASSWORD);
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
