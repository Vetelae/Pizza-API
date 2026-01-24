using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Pizza_API.Entities;

namespace Pizza_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Category> Categories { get; set; }

        // Try adding this method
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); 
        }
    }
}