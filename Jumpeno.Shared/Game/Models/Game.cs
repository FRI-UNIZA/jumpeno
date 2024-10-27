namespace Jumpeno.Shared.Models;

public class Game {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const byte CODE_LENGTH = 5;

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public static List<string> ValidateCode(string code) {
        var errors = new List<string>();
        if (code.Length != CODE_LENGTH) errors.Add(@I18N.T("Length must be equal to I18N{length}", new() { { "length", $"{CODE_LENGTH}" } }));
        if (!Checker.IsAlphaNum(code)) errors.Add(@I18N.T("Code must be alphanumeric"));
        if (code.ToUpper() != code) errors.Add(@I18N.T("Code must be uppercase"));
        return errors;
    }
    public static void CheckCode(string code) {
        var errors = ValidateCode(code);
        if (errors.Count > 0) throw new ArgumentException(errors[0]);
    }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public string Code { get; private set; }
    public double Time { get; private set; }
    public GAME_STATE State { get; private set; }
    public List<Player> Players { get; private set; }
}
