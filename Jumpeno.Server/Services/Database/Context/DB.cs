namespace Jumpeno.Server.Services;

public class DB : DbContext {
    // Constants --------------------------------------------------------------------------------------------------------------------------
    public static readonly string Dir = Path.Join("Services", "Database");
    public static readonly string FileName = "Jumpeno.db";
    public static readonly string FilePath = Path.Join(Dir, FileName);
    public static readonly string ConnectionString = $"Data Source={FilePath}";

    // Tables -----------------------------------------------------------------------------------------------------------------------------
    public DbSet<Person> Persons { get; set; }
    
    // Lifecycle --------------------------------------------------------------------------------------------------------------------------
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite(ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Person>().ToTable("Persons");
        modelBuilder.Entity<Person>()
            .HasData(
                new Person {
                    Id = 3,
                    Email = "Fero.Mrkva@gmail.com",
                    Name = "Fero",
                    Skin = "Red",
                    ActivationCode = "5678"
                }
            );
    }
}
