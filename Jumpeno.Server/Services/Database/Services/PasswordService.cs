namespace Jumpeno.Server.Services;

using System.Security.Cryptography;

public class PasswordService(DB dbContext, TransactionService transactionService)
{
    public const int HashIterations = 100_000;

    // Utils ------------------------------------------------------------------------------------------------------------------------------
    public string Pepper(string password) => $"{password}{ServerSettings.Auth.Pepper}";

    public byte[] GenerateSalt() => RandomNumberGenerator.GetBytes(PasswordEntity.SaltSize);

    public byte[] HashPassword(string password, byte[] salt) => Rfc2898DeriveBytes.Pbkdf2(
        Pepper(password),
        salt,
        HashIterations,
        HashAlgorithmName.SHA256,
        PasswordEntity.HashSize
    );

    public bool Validate(string password, byte[] salt, byte[] hash)
        => CryptographicOperations.FixedTimeEquals(HashPassword(password, salt), hash);

    // Create -----------------------------------------------------------------------------------------------------------------------------
    public async Task<PasswordEntity> Create(
        // Parameters:
        string id, string password,
        // Exceptions:
        string idID = "", string passwordID = ""
    )
    {
        // 1) Validation:
        var errors = UserValidator.ValidateID(id, idID);
        errors.AddRange(UserValidator.ValidatePassword(password, passwordID));
        Checker.Assert(errors, Exceptions.Values);
        // 2) Create record:
        var salt = GenerateSalt();
        var record = new PasswordEntity()
        {
            ID = id,
            User = null!,
            Hash = HashPassword(password, salt),
            Salt = salt,
            ModifiedAt = DateTime.UtcNow
        };
        // 3) Save record:
        dbContext.Password.Add(record);
        await transactionService.SaveWithDuplicationCheck();
        // 4) Return record:
        return record;
    }

    // Update -----------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Update(
        // Parameters:
        string id, string password,
        // Exceptions:
        string idID = "", string passwordID = ""
    )
    {
        // 1) Validation:
        var errors = UserValidator.ValidateID(id, idID);
        errors.AddRange(UserValidator.ValidatePassword(password, passwordID));
        Checker.Assert(errors, Exceptions.Values);
        // 2) Update record:
        var salt = GenerateSalt();
        var rows = await dbContext.Password
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
    public async Task<bool> Delete(
        // Parameters:
        string id,
        // Exceptions:
        string idID = ""
    )
    {
        // 1) Validation:
        UserValidator.AssertID(id, idID);
        // 2) Delete record:
        var rows = await dbContext.Password
            .Where(o => o.ID == id)
            .ExecuteDeleteAsync();
        // 3) True if deleted:
        return rows > 0;
    }

    // Read -------------------------------------------------------------------------------------------------------------------------------
    public async Task<PasswordEntity?> ByID(
        // Parameters:
        string id,
        // Exceptions:
        string idID = ""
    )
    {
        // 1) Validation:
        UserValidator.AssertID(id, idID);

        // 2) Select record:
        var record = await dbContext.Password
            .FirstOrDefaultAsync(o => o.ID == id);

        // 3) Return record:
        return record;
    }

    public async Task<PasswordEntity?> ByIDLeftJoinUserLeftJoinRefresh(
        // Parameters:
        string id,
        // Exceptions:
        string idID = ""
    )
    {
        // 1) Validation:
        UserValidator.AssertID(id, idID);

        // 2) Select record:
        var record = await dbContext.Password
            .Include(o => o.User)
                .ThenInclude(u => u.Refresh)
            .FirstOrDefaultAsync(o => o.ID == id);

        // 3) Return record:
        return record;
    }

    public async Task<(PasswordEntity, IEnumerable<RefreshEntity>)?> ByIDLeftJoinRefresh(
        // Parameters:
        string id,
        // Exceptions:
        string idID = ""
    )
    {
        // 1) Validation:
        UserValidator.AssertID(id, idID);

        // 2) Select record:
        var record = await dbContext.Password
            .GroupJoin(
                dbContext.Refresh,
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
