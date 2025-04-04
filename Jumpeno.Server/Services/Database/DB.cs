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

    // Tables -----------------------------------------------------------------------------------------------------------------------------
    public DbSet<EntityUser> User { get; set; }
    public DbSet<EntityActivation> Activation { get; set; }

    // Configuration ----------------------------------------------------------------------------------------------------------------------
    public static void Setup(DbContextOptionsBuilder options) =>
    options.UseMySql(CONNECTION_STRING, new MySqlServerVersion(new Version(VERSION)));
    protected override void OnConfiguring(DbContextOptionsBuilder options) => Setup(options);

    // Context ----------------------------------------------------------------------------------------------------------------------------
    public static async Task<DB> Context() { 
        // 1.1) Get context:
        var ctx = RequestStorage.Get<DB>(nameof(DB));
        if (ctx != null) return ctx;
        // 1.2) Create a new context:
        ctx = await AppEnvironment.GetService<IDbContextFactory<DB>>().CreateDbContextAsync();
        RequestStorage.Set(nameof(DB), ctx);
        Disposer.RequestRegister(ctx);
        return ctx;
    }
}
