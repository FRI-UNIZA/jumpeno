namespace Jumpeno.Server.Services;

public class ActivationService(DB dbContext, TransactionService transactionService)
{
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly int EXPIRES = From.HourToMS(ServerSettings.Expiration.ActivationToken.Hours); // ms

    // Create -----------------------------------------------------------------------------------------------------------------------------
    public async Task<ActivationEntity> Create(
        // Parameters:
        string id,
        // Exceptions:
        string idID = ""
    )
    {
        // 1) Validation:
        UserValidator.AssertID(id, idID);
        // 2) Create record:
        var record = new ActivationEntity()
        {
            ID = id,
            User = null!
        };
        // 3) Save record:
        dbContext.Activation.Add(record);
        await transactionService.SaveWithDuplicationCheck();
        // 4) Return record:
        return record;
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
        var rows = await dbContext.Activation
            .Where(o => o.ID == id)
            .ExecuteDeleteAsync();
        // 3) True if deleted:
        return rows > 0;
    }

    public async Task<bool> DeleteExpired()
    {
        // 1) Delete records:
        var rows = await dbContext.User
            .Where(o => o.Activation != null)
            .Where(o => o.CreatedAt < DateTime.UtcNow.AddMilliseconds(-EXPIRES))
            .ExecuteDeleteAsync();
        // 2) True if deleted:
        return rows > 0;
    }
}
