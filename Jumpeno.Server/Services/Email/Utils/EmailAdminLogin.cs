namespace Jumpeno.Server.Services;

public static partial class Email {
    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void SendAdminLogin(string email, string refreshToken) {
        var q = new QueryParams(); q.Set(TOKEN_TYPE.REFRESH.String(), refreshToken);
        Send(
            email,
            I18N.T("Jumpeno login"),
            EMAIL_CONTENT.LINK(
                I18N.T("Jumpeno login"),
                I18N.T("Hello, here is your login link:"),
                I18N.T("Log in"),
                URL.ToAbsolute(URL.SetQueryParams(I18N.Link<LoginPage>(), q))
            )
        );
    }
}
