namespace Jumpeno.Shared.Services;

public static class ImageReferrer {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, Func<ElementReference>> Refs = [];

    // Methods ----------------------------------------------------------------------------------------------------------------------------
    public static void SetRef(string url, Func<ElementReference> getter) {
        Refs[url] = getter;
    }

    public static ElementReference? GetRef(string url) {
        Refs.TryGetValue(url, out var getter);
        if (getter == null) return null;
        return getter();
    }
}
