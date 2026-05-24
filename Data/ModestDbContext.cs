using Microsoft.EntityFrameworkCore;
using Modesta.Models;
using BCrypt.Net;

namespace Modesta.Data
{
    public class ModestDbContext : DbContext
    {
        public ModestDbContext() { }

        public ModestDbContext(DbContextOptions<ModestDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<ItemTag> ItemTags { get; set; }
        public DbSet<ClothingItem> ClothingItems { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlite(@"Data Source=C:\Users\Heidi\modesta.db");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User
            {
                UserId = 1,
                Username = "admin",
                Email = "admin@modesta.be",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                IsAdmin = true,
                PrivacySetting = "private",
                UITheme = "beige",
                CreatedAt = System.DateTime.Now
            });
        }
    }
}