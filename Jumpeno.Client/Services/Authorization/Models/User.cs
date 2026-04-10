namespace Jumpeno.Client.Models;

public class User {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly string[] DEFAULT_NAMES = [
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
    ];
    public const string NAME_UNKNOWN = "Unknown";
    public static readonly User UNKNOWN = new(NAME_UNKNOWN);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Guid? ID { get; private set; }
    public string? Email { get; private set; }
    public string Name { get; private set; }
    public Skin Skin { get; set; }
    public bool Activated { get; private set; }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool Equals(User? user) {
        if (user == null) return false;
        if (ID == null && user.ID == null) return user.Name == Name;
        return user.ID == ID;
    }

    // Generators -------------------------------------------------------------------------------------------------------------------------
    public static Guid GenerateID() => Guid.NewGuid();
    public static string GenerateName() => DEFAULT_NAMES[new Random().Next(DEFAULT_NAMES.Length)];
    public static Skin GenerateSkin() => (Skin) new Random().Next(Enum.GetValues(typeof(Skin)).Length);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private User(Guid? id, string? email, string name, Skin skin, bool activated) {
        ID = id;
        Email = email;
        Name = UserValidator.AssertName(name, checkUnknown: false);
        Skin = UserValidator.AssertSkin(skin);
        Activated = activated;
    }
    public User(Guid id, string email, string name, Skin skin, bool activated) : this((Guid?) id, email, name, skin, activated) {}
    public User(string name) : this(null, null, name, default, true) {}
}
