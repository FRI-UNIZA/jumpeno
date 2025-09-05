namespace Jumpeno.Server.Services;

public static partial class Email {
    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void SendPasswordReset(string email, string password, string resetToken) {
        var q = new QueryParams(); q.Set(TOKEN_TYPE.PASSWORD_RESET.String(), resetToken);
        Send(
            email,
            I18N.T("Jumpeno password reset"),
            EMAIL_CONTENT.LINK(
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
