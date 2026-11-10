namespace Jumpeno.Server.Services;

using MySqlConnector;

public class DB : DbContext {

    public DB(DbContextOptions<DB> options) : base(options) { }

    // Tables -----------------------------------------------------------------------------------------------------------------------------
    public DbSet<UserEntity> User { get; set; }
    public DbSet<PasswordEntity> Password { get; set; }
    public DbSet<ActivationEntity> Activation { get; set; }
    public DbSet<RefreshEntity> Refresh { get; set; }

    
}
