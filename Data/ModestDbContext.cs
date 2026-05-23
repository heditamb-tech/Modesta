using Microsoft.EntityFrameworkCore;
using Modesta.Models;
using System.IO;

namespace Modesta.Data
{
    public class ModestDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "modesta.db");
            options.UseSqlite($"Data Source={path}");
        }

        public ModestDbContext()
        {
            Database.EnsureCreated();
        }
    }
}