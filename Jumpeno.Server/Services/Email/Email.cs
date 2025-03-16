namespace Jumpeno.Server.Services;

using MimeKit;
using MailKit.Net.Smtp;

public static class Email {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string SMTP = "smtp.ethereal.email";
    public const int PORT = 587;
    public const string FROM = "brooks.windler@ethereal.email";
    public const string PASSWORD = "UBt1fRxpFTgEBb8TDt";

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void Send(string to, string subject, string text) {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(FROM));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html) {
            Text = text,
        };
        using var smtp = new SmtpClient();
        smtp.Connect(SMTP, 587, MailKit.Security.SecureSocketOptions.StartTls);
        smtp.Authenticate(FROM, PASSWORD);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}
