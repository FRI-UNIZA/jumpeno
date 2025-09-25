namespace Jumpeno.Client.Utils;

using System;
using System.Linq;

public class StringGenerator {
    // Attributes -------------------------------------------------------------------------------------------------------------------------
    private readonly Random G = new();

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public string Generate(int minLength, int maxLength, string characters = CHARS.ALPHA_UPPER) {
        // 1) Check values:
        Checker.CheckGreaterOrEqualTo(minLength, 1, nameof(minLength));
        Checker.CheckGreaterOrEqualTo(maxLength, minLength, nameof(maxLength));
        // 2) Generate:
        int length = G.Next(minLength, maxLength + 1);
        // 3) Convert to string:
        return new string([.. Enumerable.Range(0, length).Select(_ => characters[G.Next(characters.Length)])]);
    }
}
