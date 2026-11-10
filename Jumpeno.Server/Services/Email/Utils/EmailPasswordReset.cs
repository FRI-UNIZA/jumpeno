namespace Jumpeno.Server.Services;

public static partial class Email {
    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void SendPasswordReset(string email, string password, string resetToken) {
        var q = new QueryParams(); q.Set(TokenType.PasswordReset.String(), resetToken);
        Send(
            email,
            I18N.T("Jumpeno password reset"),
            EmailsContents.Link(
                I18N.T("Jumpeno password reset"),
                $"{I18N.T("Hello, confirm that your password can be reset to:")}"
                + "<br><br>"
                + $"<b>{password}</b>",
                I18N.T("Confirm reset"),
                URL.ToAbsolute(URL.SetQueryParams(I18N.Link<LoginPage>(), q))
            )
        );
    }
}
