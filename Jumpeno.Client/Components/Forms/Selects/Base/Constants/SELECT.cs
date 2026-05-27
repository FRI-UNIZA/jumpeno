namespace Jumpeno.Client.Constants;

public static class Select<T> {
    public static readonly SelectOption<T> EmptyOption = new(-1, default, I18N.T("Empty"));
}
