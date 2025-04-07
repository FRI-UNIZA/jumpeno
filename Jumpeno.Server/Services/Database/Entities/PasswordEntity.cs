namespace Jumpeno.Server.Models;

using System.Security.Cryptography;

public class PasswordEntity {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly string PEPPER = ServerSettings.Auth.Pepper;
    public const int HASH_SIZE = 32; // bytes
    public const int SALT_SIZE = 16; // bytes
    public const int HASH_ITERATIONS = 100_000;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public const string INDEX_ID = "PRIMARY";
    [Key]
    [ForeignKey(nameof(User))]
    [Column(TypeName = "VARCHAR(255)")]
    public required string ID { get; set; }
    public required UserEntity User { get; set; }

    [Column(TypeName = "BINARY(32)")]
    public required byte[] Hash { get; set; }

    [Column(TypeName = "BINARY(16)")]
    public required byte[] Salt { get; set; }

    public required DateTime ModifiedAt { get; set; }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static string Pepper(string password) => $"{password}{PEPPER}";

    public static byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(SALT_SIZE);
    
    public static byte[] HashPassword(string password, byte[] salt) => Rfc2898DeriveBytes.Pbkdf2(
        Pepper(password),
        salt,
        HASH_ITERATIONS,
        HashAlgorithmName.SHA256,
        HASH_SIZE
    );

    public static bool Validate(string password, byte[] salt, byte[] hash) {
        return CryptographicOperations.FixedTimeEquals(HashPassword(password, salt), hash);
    }

    // Actions ----------------------------------------------------------------------------------------------------------------------------
    public static async Task<PasswordEntity> Create(UserEntity user, string password) {
        // 1) Validation:
        var errors = UserValidator.ValidateID(user.ID);
        errors.AddRange(UserValidator.ValidatePassword(password));
        Checker.CheckValues(errors);
        // 2) Create record:
        var salt = GenerateSalt();
        var record = new PasswordEntity() {
            ID = user.ID,
            User = user,
            Hash = HashPassword(password, salt),
            Salt = salt,
            ModifiedAt = user.ModifiedAt
        };
        // 3) Save record:
        var ctx = await DB.Context();
        ctx.Password.Add(record);
        await DB.Save();
        // 4) Return record:
        return record;
    }
}
