namespace Jumpeno.Shared.Models;

public class User {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public const string USER_ID = "user";
    public const string NAME_ID = "user-name";

    public const byte NAME_MIN_LENGTH = 3;
    public const byte NAME_MAX_LENGTH = 13;
    public const string NAME_UNKNOWN = "Unknown";
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

    public static readonly User UNKNOWN = new(NAME_UNKNOWN, SKIN.MAGE_MAGIC);

    // Generators -------------------------------------------------------------------------------------------------------------------------
    public static Guid GenerateID() => Guid.NewGuid();
    public static string GenerateName() => DEFAULT_NAMES[new Random().Next(DEFAULT_NAMES.Length)];
    public static SKIN GenerateSkin() => (SKIN) new Random().Next(Enum.GetValues(typeof(SKIN)).Length);
    public static User GenerateUser() => new(GenerateID(), GenerateName(), GenerateSkin());

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Guid? ID { get; private set; }
    public string Name { get; private set; }
    public SKIN Skin { get; private set; }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool Equals(User? user) {
        if (user == null) return false;
        if (user.ID == null) return user.Name == Name;
        return user.ID == ID;
    }

    // Validation -------------------------------------------------------------------------------------------------------------------------
    public static List<Error> ValidateName(string name) {
        var errors = Checker.Validate(name.Length < NAME_MIN_LENGTH || NAME_MAX_LENGTH < name.Length, new Error(
            NAME_ID,
            "Length is not between I18N{min} and I18N{max}",
            new() {{"min", $"{NAME_MIN_LENGTH}"}, {"max", $"{NAME_MAX_LENGTH}"}}
        ));
        Checker.Validate(errors, !Checker.IsAlphaNum(name), new Error(NAME_ID, "Name must be alphanumeric"));
        Checker.Validate(errors, name.Length > 0 && char.IsDigit(name[0]), new Error(NAME_ID, "Name must not start with number"));
        return errors;
    }
    public static void CheckName(string name) => Checker.Check(ValidateName(name));

    public static List<Error> ValidateUnknown(User user) => Checker.Validate(UNKNOWN.Equals(user), new Error("User undefined!"));
    public static void CheckUnknown(User user) => Checker.Check(ValidateUnknown(user));

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private User(Guid? id, string name, SKIN skin) {
        CheckName(name);
        ID = id;
        Name = name;
        Skin = skin;
    }
    public User(Guid id, string name, SKIN skin) : this((Guid?) id, name, skin) {}
    public User(string name, SKIN skin) : this(null, name, skin) {}
}
