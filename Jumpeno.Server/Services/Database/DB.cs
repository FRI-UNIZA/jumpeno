namespace Jumpeno.Server.Services;

public class DB : DbContext {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly string VERSION = ServerSettings.Database.Version;
    public static readonly string HOST = ServerSettings.Database.Host;
    public static readonly int PORT = ServerSettings.Database.Port;
    public static readonly string USER = ServerSettings.Database.User;
    public static readonly string PASSWORD = ServerSettings.Database.Password;
    public static readonly string DATABASE = ServerSettings.Database.Database;
    public static readonly string CONNECTION_STRING =
          $"Server={HOST};"
        + $"Database={DATABASE};"
        + $"User={USER};"
        + $"Password={PASSWORD};"
        + $"Port={PORT}"
    ;

    // Configuration ----------------------------------------------------------------------------------------------------------------------
    public static void Setup(DbContextOptionsBuilder options) =>
    options.UseMySql(CONNECTION_STRING, new MySqlServerVersion(new Version(VERSION)));
    protected override void OnConfiguring(DbContextOptionsBuilder options) => Setup(options);

    // Constraints ------------------------------------------------------------------------------------------------------------------------
    private static List<Error> HandleUniqueConstraints(DbUpdateException e, Dictionary<string, Error>? uniqueErrors = null) {
        // 1) Check custom errors:
        if (uniqueErrors == null) throw e;
        // 2) Swap unique errors:
        List<Error> errors = [];
        Exception top = e;
        while (top.InnerException != null) {
            var inner = top.InnerException; top = inner;
            // 2.1) Check exception type:
            if (!(inner.GetType().Name.Contains("MySqlException") && inner.Message.StartsWith("Duplicate entry"))) continue;
            // 2.2) Parse key:
            var forKey = "' for key '";
            var index = inner.Message.IndexOf(forKey);
            if (index < 0) continue;
            var key = inner.Message[(index + forKey.Length)..];
            key = key[..key.IndexOf('\'')];
            // 2.3) Set error:
            if (!uniqueErrors.TryGetValue(key, out var error)) continue;
            errors.Add(error);
        }
        // 3) Throw if not swapped:
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
        // 1) Try to get existing context:
        var ctx = RequestStorage.Get<DB>(nameof(DB));
        if (ctx != null) return ctx;
        // 2) Or create a new context:
        ctx = await AppEnvironment.GetService<IDbContextFactory<DB>>().CreateDbContextAsync();
        RequestStorage.Set(nameof(DB), ctx);
        Disposer.RequestRegister(ctx);
        return ctx;
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
    // rows - rows affected
    // error key == column index name
    public static async Task<(int rows, List<Error> errors)> Save(Dictionary<string, Error>? uniqueErrors = null) {
        try {
            var ctx = await Context();
            return (await ctx.SaveChangesAsync(), []);
        } catch (DbUpdateException e) {
            return (0, HandleUniqueConstraints(e, uniqueErrors));
        }
    }
}
