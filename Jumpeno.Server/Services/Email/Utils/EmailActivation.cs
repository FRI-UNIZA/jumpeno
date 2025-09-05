namespace Jumpeno.Server.Services;

public static partial class Email {
    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static void SendActivation(string email, string id) {
        try {
            var q = new QueryParams(); q.Set(TOKEN_TYPE.ACTIVATION.String(), JWT.GenerateActivation(Guid.Parse(id)));
            Send(
                email,
                I18N.T("Jumpeno activation"),
                EMAIL_CONTENT.LINK(
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
}
