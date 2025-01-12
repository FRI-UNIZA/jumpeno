namespace Jumpeno.Shared.Services;

public static class ImageReferrer {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, Func<ElementReference>> Refs = [];

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static void Set(string url, Func<ElementReference> getter) {
        Refs[url] = getter;
    }

    public static ElementReference? Get(string url) {
        if (!Refs.TryGetValue(url, out var getter)) return null;
        return getter();
    }
}
