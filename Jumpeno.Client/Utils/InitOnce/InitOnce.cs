namespace Jumpeno.Client.Utils;

public class InitOnce {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private static readonly Dictionary<string, bool> Inits = [];

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public static void Check(string name) {
        #if IS_DEVELOPMENT
            if (Inits.ContainsKey(name)) throw new InvalidOperationException("Already initialized!");
            Inits[name] = true;
        #endif
    }
}
