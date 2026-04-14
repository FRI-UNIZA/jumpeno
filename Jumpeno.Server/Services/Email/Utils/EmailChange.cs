namespace Jumpeno.Server.Services;

public static partial class Email
{
    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void SendEmailChange(string email, string id)
    {
        try
        {
            //var q = new QueryParams(); 
            //q.Set(TokenType.EMAIL_CHANGE.String(), JWT.GenerateEmailChange(Guid.Parse(id), email));
            //Send(
            //    email,
            //    I18N.T("Jumpuno email change"),
            //    EmailsContents.LINK(
            //        I18N.T("Jumpuno email change"),
            //        I18N.T("Hello, here is your email change link:"),
            //        I18N.T("Change"),
            //        URL.ToAbsolute(URL.SetQueryParams(API.BASE.USER_EMAIL_CHANGE, q))
            //    )
            //);
        }
        catch
        {
            throw Exceptions.SERVER.SetInfo(I18N.T("Failed to send email address change email."));
        }
    }

    public static bool TrySendEmailChange(string email, string id)
    {
        try { SendEmailChange(email, id); return true; }
        catch { return false; }
    }
}
