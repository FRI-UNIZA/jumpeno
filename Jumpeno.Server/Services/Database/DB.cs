namespace Jumpeno.Server.Services;

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
    private static List<Error> HandleUniqueConstraints(DbUpdateException e, Dictionary<string, Error>? uniques = null) {
        // 1) Check custom errors:
        if (uniques == null) throw e;
        // 2) Swap unique errors:
        List<Error> errors = [];
        Exception top = e;
        while (top.InnerException != null) {
            var inner = top.InnerException; top = inner;
            // 2.1) Check exception type:
            if (!(inner.GetType().Name.Contains(MYSQL_EXCEPTION) && inner.Message.StartsWith(START_PHRASE))) continue;
            // 2.2) Parse key:
            var index = inner.Message.IndexOf(SEARCH_PHRASE);
            if (index < 0) continue;
            var key = inner.Message[(index + SEARCH_PHRASE.Length)..];
            key = key[..key.IndexOf('\'')];
            // 2.3) Set error:
            if (!uniques.TryGetValue(key, out var error)) continue;
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
        if (AppEnvironment.IsController) {
            // 1) Try to get existing context:
            var ctx = RequestStorage.Get<DB>(nameof(DB));
            if (ctx != null) return ctx;
            // 2) Or create a new context:
            ctx = await AppEnvironment.GetService<IDbContextFactory<DB>>().CreateDbContextAsync();
            RequestStorage.Set(nameof(DB), ctx);
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
    // rows - rows affected
    // errors - key == column index name
    public static async Task<(int rows, List<Error> errors)> Save(Dictionary<string, Error>? uniques = null) {
        try {
            var ctx = await Context();
            return (await ctx.SaveChangesAsync(), []);
        } catch (DbUpdateException e) {
            return (0, HandleUniqueConstraints(e, uniques));
        }
    }
}
