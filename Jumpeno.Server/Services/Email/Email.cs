namespace Jumpeno.Server.Services;

using MimeKit;
using MailKit.Net.Smtp;

public static partial class Email {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static string Host => ServerSettings.Email.Host;
    public static int Port => ServerSettings.Email.Port;
    public static string Address => AppSettings.Email;
    public static string Password => ServerSettings.Email.Password;
    public static string BackupKeys => ServerSettings.Email.BackupKeys;
    public static string AppPassword => ServerSettings.Email.AppPassword;
    public static bool Mailcatcher => ServerSettings.Email.Mailcatcher;

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void Send(string to, string subject, string content) {
        // 1) Create message:
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(Address));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Date = DateTime.UtcNow;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = content };
        // 2) Connect to SMTP server:
        using var smtp = new SmtpClient();
        if (Mailcatcher) {
            smtp.Connect(Host, Port, false);
        } else {
            smtp.Connect(Host, Port, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(Address, AppPassword);
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
