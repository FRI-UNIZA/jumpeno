namespace Jumpeno.Shared.Models;

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
    public static readonly User UNKNOWN = new(NAME_UNKNOWN, SKIN.MAGE_MAGIC);

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public Guid? ID { get; private set; }
    public string? Email { get; private set; }
    public string Name { get; private set; }
    public SKIN Skin { get; private set; }
    public bool Activated { get; private set; }

    // Predicates -------------------------------------------------------------------------------------------------------------------------
    public bool Equals(User? user) {
        if (user == null) return false;
        if (user.ID == null) return user.Name.ToLower() == Name.ToLower();
        return user.ID == ID;
    }

    // Generators -------------------------------------------------------------------------------------------------------------------------
    public static Guid GenerateID() => Guid.NewGuid();
    public static string GenerateName() => DEFAULT_NAMES[new Random().Next(DEFAULT_NAMES.Length)];
    public static SKIN GenerateSkin() => (SKIN) new Random().Next(Enum.GetValues(typeof(SKIN)).Length);

    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    [JsonConstructor]
    private User(Guid? id, string? email, string name, SKIN skin, bool activated) {
        ID = id;
        Email = email;
        Name = UserValidator.CheckName(name, null, false);
        Skin = UserValidator.CheckSkin(skin);
        Activated = activated;
    }
    public User(Guid id, string email, string name, SKIN skin, bool activated) : this((Guid?) id, email, name, skin, activated) {}
    public User(string name, SKIN skin) : this(null, null, name, skin, true) {}
}
