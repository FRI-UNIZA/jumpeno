namespace Jumpeno.Shared.Constants;

public static class API {
    public static class BASE {
        public static readonly Func<string> URL = () => $"{AppSettings.Api.Base.URL}{AppSettings.Api.Base.Prefix}";
        public static readonly Func<string> SET_CULTURE = () => $"{URL()}/Culture/Set";
        public static readonly Func<string> COOKIE = () => $"{URL()}/Cookie";
    }
}
