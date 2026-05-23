using Microsoft.EntityFrameworkCore;
using Modesta.Models;

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
    }
}