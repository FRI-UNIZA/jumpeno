namespace Jumpeno.Server.Services;

public class RefreshService(DB dbContext, TransactionService transactionService)
{
    // Create -----------------------------------------------------------------------------------------------------------------------------
    public async Task<RefreshEntity> Create(
        // Parameters:
        string token, string? id = null, string? origin = null,
        // Exceptions:
        string tokenID = "", string idId = "", string originId = ""
    )
    {
        // 1) Validation:
        var errors = TokenValidator.ValidateToken(token, tokenID);
        if (id != null) errors.AddRange(UserValidator.ValidateID(id, idId));
        errors.AddRange(Checker.Validate(token == origin, Errors.Match(nameof(token), nameof(origin)).SetID(originId)));
        Checker.Assert(errors, Exceptions.Values);
        // 2) Read token:
        var data = Token.Decode(token) ?? throw Exceptions.NotAuthenticated;
        if (id != null && id != data.sub) throw new InvalidDataException(nameof(UserEntity.Id));
        // 3) Create record:
        var record = new RefreshEntity()
        {
            Token = token,
            Id = id,
            Origin = origin,
            Expires = data.exp
        };
        // 4) Save record:
        dbContext.Refresh.Add(record);
        await transactionService.SaveWithDuplicationCheck();
        // 5) Return record:
        return record;
    }

    // Read -------------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> IsValid(
        // Parameters:
        string token,
        // Exceptions:
        string tokenID = ""
    )
    {
        // 1) Validation:
        TokenValidator.AssertToken(token, tokenID);
        // 2) Select record:
        var record = await dbContext.Refresh
            .FirstOrDefaultAsync(
                o => o.Token == token
                && o.Expires > DateTime.UtcNow
            );
        // 3) True if valid:
        return record != null;
    }

    public async Task<RefreshEntity?> ByToken(
        // Parameters:
        string token,
        // Exceptions:
        string tokenID = ""
    )
    {
        // 1) Validation:
        TokenValidator.AssertToken(token, tokenID);
        // 2) Select record:
        var record = await dbContext.Refresh
            .FirstOrDefaultAsync(o => o.Token == token);
        // 3) Return record:
        return record;
    }

    // Delete -----------------------------------------------------------------------------------------------------------------------------
    public async Task<bool> Delete(
        // Parameters:
        string token,
        // Exceptions:
        string tokenID = ""
    )
    {
        // 1) Validation:
        TokenValidator.AssertToken(token, tokenID);
        // 2) Delete record:
        var rows = await dbContext.Refresh
            .Where(o => o.Token == token)
            .ExecuteDeleteAsync();
        // 3) True if deleted:
        return rows > 0;
    }

    public async Task<bool> DeleteByOrigin(
        // Parameters:
        string origin, string? except = null,
        // Exceptions:
        string originID = "", string exceptId = ""
    )
    {
        // 1) Validation:
        var errors = TokenValidator.ValidateToken(origin, originID);
        if (except != null) errors.AddRange(TokenValidator.ValidateToken(except, exceptId));
        Checker.Assert(errors, Exceptions.Values);
        // 2) Delete records:
        var rows = await dbContext.Refresh
            .Where(o => o.Origin == origin)
            .Where(o => o.Token != except)
            .ExecuteDeleteAsync();
        // 3) True if deleted:
        return rows > 0;
    }

    public async Task<bool> DeleteExpired()
    {
        // 1) Delete records:
        var rows = await dbContext.Refresh
            .Where(o => o.Expires <= DateTime.UtcNow)
            .ExecuteDeleteAsync();
        // 2) True if deleted:
        return rows > 0;
    }

    public async Task<bool> DeleteByUserID(
        // Parameters:
        string id,
        // Exceptions:
        string idId = ""
    )
    {
        // 1) Validation:
        UserValidator.AssertID(id, idId);
        // 2) Delete records:
        var rows = await dbContext.Refresh
            .Where(o => o.Id == id)
            .ExecuteDeleteAsync();
        // 3) True if deleted:
        return rows > 0;
    }
}
