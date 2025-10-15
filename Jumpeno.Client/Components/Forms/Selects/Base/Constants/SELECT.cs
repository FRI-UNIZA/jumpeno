namespace Jumpeno.Client.Constants;

public static class SELECT<T> {
    public static readonly SelectOption<T> EMPTY_OPTION = new(-1, default, I18N.T("Empty"));
}
