namespace Jumpeno.Server.Models;

using System.Security.Cryptography;

public class PasswordEntity {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly string PEPPER = ServerSettings.Auth.Pepper;
    public const int HASH_SIZE = 32; // Bytes
    public const int SALT_SIZE = 16; // Bytes
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

    public static bool Validate(string password, byte[] salt, byte[] hash)
        => CryptographicOperations.FixedTimeEquals(HashPassword(password, salt), hash);

    // Create -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<PasswordEntity> Create(
        string id, string password,
        string idID = "", string passwordID = ""
    ) {
        // 1) Validation:
        var errors = UserValidator.ValidateID(id, idID);
        errors.AddRange(UserValidator.ValidatePassword(password, passwordID));
        Checker.Assert(errors, EXCEPTION.VALUES);
        // 2) Create record:
        var salt = GenerateSalt();
        var record = new PasswordEntity() {
            ID = id,
            User = null!,
            Hash = HashPassword(password, salt),
            Salt = salt,
            ModifiedAt = DateTime.UtcNow
        };
        // 3) Save record:
        var ctx = await DB.Context();
        ctx.Password.Add(record);
        await DB.Save();
        // 4) Return record:
        return record;
    }

    // Update -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<bool> Update(
        string id, string password,
        string idID = "", string passwordID = ""
    ) {
        // 1) Validation:
        var errors = UserValidator.ValidateID(id, idID);
        errors.AddRange(UserValidator.ValidatePassword(password, passwordID));
        Checker.Assert(errors, EXCEPTION.VALUES);
        // 2) Update record:
        var ctx = await DB.Context();
        var salt = GenerateSalt();
        int rows = await ctx.Password
            .Where(o => o.ID == id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(o => o.Hash, value => HashPassword(password, salt))
                .SetProperty(o => o.Salt, value => salt)
                .SetProperty(o => o.ModifiedAt, value => DateTime.UtcNow)
            );
        // 3) True if updated:
        return rows > 0;
    }
}
