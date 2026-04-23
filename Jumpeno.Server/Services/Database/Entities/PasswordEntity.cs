namespace Jumpeno.Server.Models;

using System.Security.Cryptography;

public class PasswordEntity {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly string PEPPER = ServerSettings.Auth.Pepper;
    public const int HashSize = 32; // Bytes
    public const int SaltSize = 16; // Bytes
    public const int HashIterations = 100_000;

    // Attributes -------------------------------------------------------------------------------------------------------------------------
    public const string IndexId = "PRIMARY";
    [Key]
    [ForeignKey(nameof(User))]
    [Column(TypeName = "VARCHAR(255)")]
    public required string ID { get; set; }

    [Column(TypeName = "BINARY(32)")]
    public required byte[] Hash { get; set; }

    [Column(TypeName = "BINARY(16)")]
    public required byte[] Salt { get; set; }

    public required DateTime ModifiedAt { get; set; }

    // Relations --------------------------------------------------------------------------------------------------------------------------
    public required UserEntity User { get; set; }

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public static string Pepper(string password) => $"{password}{PEPPER}";

    public static byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(SaltSize);
    
    public static byte[] HashPassword(string password, byte[] salt) => Rfc2898DeriveBytes.Pbkdf2(
        Pepper(password),
        salt,
        HashIterations,
        HashAlgorithmName.SHA256,
        HashSize
    );

    public static bool Validate(string password, byte[] salt, byte[] hash)
        => CryptographicOperations.FixedTimeEquals(HashPassword(password, salt), hash);

    // Create -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<PasswordEntity> Create(
        // Parameters:
        string id, string password,
        // Exceptions:
        string idID = "", string passwordID = ""
    ) {
        // 1) Validation:
        var errors = UserValidator.ValidateID(id, idID);
        errors.AddRange(UserValidator.ValidatePassword(password, passwordID));
        Checker.Assert(errors, Exceptions.Values);
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
        // Parameters:
        string id, string password,
        // Exceptions:
        string idID = "", string passwordID = ""
    ) {
        // 1) Validation:
        var errors = UserValidator.ValidateID(id, idID);
        errors.AddRange(UserValidator.ValidatePassword(password, passwordID));
        Checker.Assert(errors, Exceptions.Values);
        // 2) Update record:
        var ctx = await DB.Context();
        var salt = GenerateSalt();
        int rows = await ctx.Password
            .Where(o => o.ID == id)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(o => o.Hash, HashPassword(password, salt))
                .SetProperty(o => o.Salt, salt)
                .SetProperty(o => o.ModifiedAt, DateTime.UtcNow)
            );
        // 3) True if updated:
        return rows > 0;
    }

    // Delete -----------------------------------------------------------------------------------------------------------------------------
    public static async Task<bool> Delete(
        // Parameters:
        string id,
        // Exceptions:
        string idID = ""
    ) {
        // 1) Validation:
        UserValidator.AssertID(id, idID);
        // 2) Delete record:
        var ctx = await DB.Context();
        int rows = await ctx.Password
            .Where(o => o.ID == id)
            .ExecuteDeleteAsync();
        // 3) True if deleted:
        return rows > 0;
    }

    // Read -------------------------------------------------------------------------------------------------------------------------------
    public static async Task<PasswordEntity?> ByID(
        // Parameters:
        string id,
        // Exceptions:
        string idID = ""
    ) {
        // 1) Validation:
        UserValidator.AssertID(id, idID);

        // 2) Select record:
        var ctx = await DB.Context();
        var record = await ctx.Password
            .FirstOrDefaultAsync(o => o.ID == id);

        // 3) Return record:
        return record;
    }

    public static async Task<PasswordEntity?> ByIDLeftJoinUserLeftJoinRefresh(
        // Parameters:
        string id,
        // Exceptions:
        string idID = ""
    ) {
        // 1) Validation:
        UserValidator.AssertID(id, idID);

        // 2) Select record:
        var ctx = await DB.Context();
        var record = await ctx.Password
            .Include(o => o.User)
                .ThenInclude(u => u.Refresh)
            .FirstOrDefaultAsync(o => o.ID == id);

        // 3) Return record:
        return record;
    }

    public static async Task<(PasswordEntity, IEnumerable<RefreshEntity>)?> ByIDLeftJoinRefresh(
        // Parameters:
        string id,
        // Exceptions:
        string idID = ""
    ) {
        // 1) Validation:
        UserValidator.AssertID(id, idID);

        // 2) Select record:
        var ctx = await DB.Context();
        var record = await ctx.Password
            .GroupJoin(
                ctx.Refresh,
                password => password.ID,
                refresh => refresh.Id,
                (password, refreshes) => new { Password = password, Refresh = refreshes }
            )
            .Where(x => x.Password.ID == id)
            .FirstOrDefaultAsync();

        // 3) Return record:
        return record == null ? null : (record.Password, record.Refresh);
    }
}
