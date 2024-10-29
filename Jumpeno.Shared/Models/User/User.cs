namespace Jumpeno.Shared.Models;

public class User {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string USER_ID = "user";
    public const string NAME_ID = "user-name";
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
    public static Guid GenerateID() { return Guid.NewGuid(); }
    public static string GenerateName() { return DEFAULT_NAMES[new Random().Next(DEFAULT_NAMES.Length)]; }
    public static SKIN GenerateSkin() { return (SKIN) new Random().Next(Enum.GetValues(typeof(SKIN)).Length); }
    public static User GenerateUser() { return new User(GenerateID(), GenerateName(), GenerateSkin()); }

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateName(string name) {
        var errors = new List<Error>();
        if (name.Length < NAME_MIN_LENGTH || NAME_MAX_LENGTH < name.Length) errors.Add(new(
            NAME_ID,
            I18N.T("Length is not between I18N{min} and I18N{max}", new() { { "min", $"{NAME_MIN_LENGTH}" }, { "max", $"{NAME_MAX_LENGTH}" } })
        ));
        if (!Checker.IsAlphaNum(name)) errors.Add(new(NAME_ID, I18N.T("Name must be alphanumeric")));
        if (name.Length > 0 && char.IsDigit(name[0])) errors.Add(new(NAME_ID, I18N.T("Name must not start with number")));
        return errors;
    }
    public static void CheckName(string name) {
        var errors = ValidateName(name);
        if (errors.Count > 0) throw new ArgumentException(errors[0].Message);
    }

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Guid? ID { get; private set; }
    public string Name { get; private set; }
    public SKIN Skin { get; private set; }

    // Constructors -----------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private User(Guid? id, string name, SKIN skin) {
        CheckName(name);
        ID = id;
        Name = name;
        Skin = skin;
    }
    public User(Guid id, string name, SKIN skin): this((Guid?) id, name, skin) {}
    public User(string name, SKIN skin): this(null, name, skin) {}
}
