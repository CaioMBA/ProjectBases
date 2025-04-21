using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Database.EntityFrameworkContexts
{
    public class AppDbContext : DbContext
    {
        private readonly string _dbPath = Path.Combine(Directory.GetCurrentDirectory(), "default_app.db");

        public DbSet<LogEntity> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite($"Filename={_dbPath}", b => b.MigrationsAssembly("Data"));
            }
        }
    }
}
