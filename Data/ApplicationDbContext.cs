using Microsoft.EntityFrameworkCore;
using Pizza_API.Entities;

namespace Pizza_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}