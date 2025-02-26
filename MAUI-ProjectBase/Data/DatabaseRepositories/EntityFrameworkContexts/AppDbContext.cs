using Domain;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.DatabaseRepositories.EntityFrameworkContexts
{
    //TO USE MIGRATION|UPDATE DATABASE COMMAND COMMENT ANDROID TARGET ON APPUI.CSPROJ
    public class AppDbContext : DbContext
    {
        private readonly string _dbPath;

        public AppDbContext(AppUtils? utils = null)
        {
            _dbPath = (utils is not null ?
                        Path.Combine(utils.GetSystemFilePath(), "default_app.db")
                            : Path.Combine(Directory.GetCurrentDirectory(), "default_app.db"));
        }

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
