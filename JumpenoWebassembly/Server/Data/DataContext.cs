using JumpenoWebassembly.Shared.ErrorHandling;
using JumpenoWebassembly.Shared.Jumpeno.Utilities;
using JumpenoWebassembly.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace JumpenoWebassembly.Server.Data
{
    /// <summary>
    /// Databaza pre Jumpeno.
    /// Obsahuje tabulku pre pouzivatelov a mapy
    /// </summary>
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { Database.Migrate(); }

        public DbSet<User> Users { get; set; }
        public DbSet<MapTemplate> Maps { get; set; }
        public DbSet<MeasurePoint> Statistics { get; set; }
        public DbSet<Error> Errors { get; set; }
        public DbSet<UserStatistics> UserStatistics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
            .HasOne(a => a.Statistics)
            .WithOne(a => a.User)
            .HasForeignKey<UserStatistics>(c => c.UserId);
        }
    }
}
