namespace Jumpeno.Shared.Models;

public class User {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const byte NAME_MIN_LENGTH = 3;
    public const byte NAME_MAX_LENGTH = 16;
    public static readonly string[] DEFAULT_NAMES = {
        "Whistlejacket",
        "Niatross",
        "Exterminator",
        "Sunline",
        "Buckpasser",
        "Ajax",
        "Crisp",
        "Longfellow",
        "Nugget",
        "Inky",
        "Joker",
        "Kermit",
        "Blink",
        "Bar",
        "Bus",
        "Azzor",
        "Jumper",
        "Stonks"
    };

    // Generators -------------------------------------------------------------------------------------------------------------------------
    public static string GenerateName() { return DEFAULT_NAMES[new Random().Next(DEFAULT_NAMES.Length)]; }

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public static List<string> ValidateName(string name) {
        var errors = new List<string>();
        if (name.Length < NAME_MIN_LENGTH || NAME_MAX_LENGTH < name.Length) errors.Add(I18N.T(
            "Length is not between I18N{min} and I18N{max}", new() { { "min", $"{NAME_MIN_LENGTH}" }, { "max", $"{NAME_MAX_LENGTH}" } }
        ));
        if (!Checker.IsAlphaNum(name)) errors.Add(I18N.T("Name must be alphanumeric"));
        if (name.Length > 0 && char.IsDigit(name[0])) errors.Add(I18N.T("Name must not start with number"));
        return errors;
    }
    public static void CheckName(string name) {
        var errors = ValidateName(name);
        if (errors.Count > 0) throw new ArgumentException(errors[0]);
    }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Guid ID { get; private set; }
    public string Name { get; private set; }
    public SKIN Skin { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    public User(Guid id, string name, SKIN skin) {
        CheckName(name);
        ID = id;
        Name = name;
        Skin = skin;
    }
}
