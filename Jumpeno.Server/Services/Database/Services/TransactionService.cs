using MySqlConnector;

namespace Jumpeno.Server.Services;

public class TransactionService(DB dbContext)
{

    // Transaction ------------------------------------------------------------------------------------------------------------------------
    public async Task Transaction(Func<Task> action, Isolation isolation = Isolation.ReadCommitted)
    {
        using var transaction = await dbContext.Database.BeginTransactionAsync((System.Data.IsolationLevel)isolation);
        try
        {
            await action();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // Save -------------------------------------------------------------------------------------------------------------------------------
    /// <summary>[Use with CREATE only!] Saves database changes and returns duplicate errors if any.</summary>
    /// <param name="uniques">Dictionary of errors to throw for column index specified as key</param>
    /// <returns>Tuple of rows affected and occured duplicate errors</returns>
    public async Task<(int rows, List<Error> errors)> SaveWithDuplicationCheck(Dictionary<string, Error>? uniques = null)
    {
        try
        {
            return (await dbContext.SaveChangesAsync(), []);
        }
        catch (DbUpdateException e)
        {
            return (0, HandleUniqueConstraints(e, uniques));
        }
    }

    /// <summary>[Use with UPDATE only!] Performs database update and returns duplicate errors if any.</summary>
    /// <param name="action">Update action to perform</param>
    /// <param name="uniques">Dictionary of errors to throw for column index specified as key</param>
    /// <returns>Tuple of rows affected and occured duplicate errors</returns> 
    public async Task<(int rows, List<Error> errors)> UpdateWithDuplicationCheck(Func<Task<int>> action, Dictionary<string, Error>? uniques = null)
    {
        try
        {
            return (await action(), []);
        }
        catch (MySqlException e)
        {
            return (0, HandleUniqueConstraints(e, uniques));
        }
    }

    // Constraints ------------------------------------------------------------------------------------------------------------------------
    private const string SearchPhrase = "' for key '";

    private Error? ParseForDuplicate(MySqlException e, Dictionary<string, Error> uniques)
    {
        // 1) Check exception type:
        if (e.ErrorCode != MySqlErrorCode.DuplicateKeyEntry) return null;
        // 2) Parse key:
        var index = e.Message.IndexOf(SearchPhrase);
        if (index < 0) return null;
        var key = e.Message[(index + SearchPhrase.Length)..];
        var end = key.IndexOf('\'');
        if (end < 0) return null;
        key = key[..end];
        if (key.Contains('.')) key = key[(key.LastIndexOf('.') + 1)..];
        // 3) Set error:
        if (!uniques.TryGetValue(key, out var error)) return null;
        return error;
    }

    private List<Error> HandleUniqueConstraints(Exception e, Dictionary<string, Error>? uniques = null)
    {
        // 1) Check custom errors:
        if (uniques == null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e).Throw();
            throw e;
        }

        // 2) Parse unique errors:
        List<Error> errors = [];
        Exception? inner = e;
        while (inner != null)
        {
            if (inner is MySqlException mySqlException)
            {
                var error = ParseForDuplicate(mySqlException, uniques);
                if (error != null)
                    errors.Add(error);
            }

            inner = inner.InnerException;
        }
        // 3) Throw if not parsed:
        if (errors.Count == 0)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e).Throw();
            throw e;
        }

        return errors;
    }
}
