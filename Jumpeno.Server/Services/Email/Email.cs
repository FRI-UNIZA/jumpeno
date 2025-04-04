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

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void Send(string to, string subject, string text) {
        // 1) Create message:
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(ADDRESS));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Date = DateTime.UtcNow;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = text };
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
}
