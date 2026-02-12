namespace Jumpeno.Server.Services;

using MySqlConnector;

public class DB : DbContext {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly string VERSION = ServerSettings.Database.Version;
    public static readonly string CONNECTION_STRING = ServerSettings.Database.ConnectionString;

    // Configuration ----------------------------------------------------------------------------------------------------------------------
    public static void Setup(DbContextOptionsBuilder options) =>
    options.UseMySql(CONNECTION_STRING, new MySqlServerVersion(new Version(VERSION)));
    protected override void OnConfiguring(DbContextOptionsBuilder options) => Setup(options);

    // Constraints ------------------------------------------------------------------------------------------------------------------------
    private const string MYSQL_EXCEPTION = "MySqlException";
    private const string START_PHRASE = "Duplicate entry";
    private const string SEARCH_PHRASE = "' for key '";

    private static Error? ParseForDuplicates(Exception e, Dictionary<string, Error> uniques) {
        // 1) Check exception type:
        if (!(e.GetType().Name.Contains(MYSQL_EXCEPTION) && e.Message.StartsWith(START_PHRASE))) return null;
        // 2) Parse key:
        var index = e.Message.IndexOf(SEARCH_PHRASE);
        if (index < 0) return null;
        var key = e.Message[(index + SEARCH_PHRASE.Length)..];
        key = key[..key.IndexOf('\'')];
        // 3) Set error:
        if (!uniques.TryGetValue(key, out var error)) return null;
        return error;
    }

    private static List<Error> HandleUniqueConstraints(DbUpdateException e, Dictionary<string, Error>? uniques = null) {
        // 1) Check custom errors:
        if (uniques == null) throw e;
        // 2) Parse unique errors:
        List<Error> errors = [];
        Exception top = e;
        while (top.InnerException != null) {
            var inner = top.InnerException; top = inner;
            var error = ParseForDuplicates(inner, uniques);
            if (error == null) continue;
            errors.Add(error);
        }
        // 3) Throw if not parsed:
        if (errors.Count == 0) throw e;
        return errors;
    }

    private static List<Error> HandleUniqueConstraints(MySqlException e, Dictionary<string, Error>? uniques = null) {
        // 1) Check custom errors:
        if (uniques == null) throw e;
        // 2) Parse unique errors:
        List<Error> errors = [];
        Exception? inner = e;
        while (inner != null) {
            var error = ParseForDuplicates(inner, uniques);
            if (error == null) continue;
            errors.Add(error);
            inner = inner.InnerException;
        }
        // 3) Throw if not parsed:
        if (errors.Count == 0) throw e;
        return errors;
    }

    // Tables -----------------------------------------------------------------------------------------------------------------------------
    public DbSet<UserEntity> User { get; set; }
    public DbSet<PasswordEntity> Password { get; set; }
    public DbSet<ActivationEntity> Activation { get; set; }
    public DbSet<RefreshEntity> Refresh { get; set; }

    // Context ----------------------------------------------------------------------------------------------------------------------------
    public static async Task<DB> Context() {
        if (AppEnvironment.IsController) {
            // 1) Try to get existing context:
            var ctx = RequestStorage.Get<DB>(REQUEST_STORAGE.DB);
            if (ctx != null) return ctx;
            // 2) Or create a new context:
            ctx = await AppEnvironment.GetService<IDbContextFactory<DB>>().CreateDbContextAsync();
            RequestStorage.Set(REQUEST_STORAGE.DB, ctx);
            Disposer.RequestRegister(ctx);
            return ctx;
        } else {   
            // 3) Server fallback (no HttpContext/RequestStorage):
            return ServerContext;
        }
    }

    // Server context ---------------------------------------------------------------------------------------------------------------------
    // NOTE: Autonomous server database operations must run in UseServerContext hook
    private static DB ServerContext = null!;
    private static readonly Locker ServerContextLock = new();
    public static async Task UseServerContext(Func<Task> action) {
        await ServerContextLock.Exclusive(async () => {
            try {
                ServerContext = await AppEnvironment.GetService<IDbContextFactory<DB>>().CreateDbContextAsync();
                await action();
            } finally {
                ServerContext.Dispose();
                ServerContext = null!;
            }
        });
    }

    // Transaction ------------------------------------------------------------------------------------------------------------------------
    public static async Task Transaction(Func<Task> action, ISOLATION isolation = ISOLATION.READ_COMMITED) {
        var db = await Context();
        using var transaction = await db.Database.BeginTransactionAsync((System.Data.IsolationLevel) isolation);
        try {
            await action();
            await transaction.CommitAsync();
        } catch {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // Save -------------------------------------------------------------------------------------------------------------------------------
    /// <summary>[Use with CREATE only!] Saves database changes and returns duplicate errors if any.</summary>
    /// <param name="uniques">Dictionary of errors to throw for column index specified as key</param>
    /// <returns>Tuple of rows affected and occured duplicate errors</returns>
    public static async Task<(int rows, List<Error> errors)> Save(Dictionary<string, Error>? uniques = null) {
        try {
            var ctx = await Context();
            return (await ctx.SaveChangesAsync(), []);
        } catch (DbUpdateException e) {
            return (0, HandleUniqueConstraints(e, uniques));
        }
    }
    
    /// <summary>[Use with UPDATE only!] Performs database update and returns duplicate errors if any.</summary>
    /// <param name="action">Update action to perform</param>
    /// <param name="uniques">Dictionary of errors to throw for column index specified as key</param>
    /// <returns>Tuple of rows affected and occured duplicate errors</returns> 
    public static async Task<(int rows, List<Error> errors)> Update(Func<Task<int>> action, Dictionary<string, Error>? uniques = null) {
        try {
            return (await action(), []);
        } catch (MySqlException e) {
            return (0, HandleUniqueConstraints(e, uniques));
        }
    }
}
