using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizza_API.Constants;
using Pizza_API.Entities;

namespace Pizza_API.Configurations
{
    public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
    {
        public void Configure(EntityTypeBuilder<MenuItem> builder)
        {
            builder.HasMany<CartItem>()
                .WithOne(ci => ci.MenuItem)
                .HasForeignKey(ci => ci.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<OrderItem>()
                .WithOne(oi => oi.MenuItem)
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(m => m.Price)
                .HasPrecision(18, 2);

            builder.Property(m => m.Name)
                .HasMaxLength(MenuItemConstraints.NameMaxLength)
                .IsRequired();

            builder.Property(m => m.Description)
                .HasMaxLength(MenuItemConstraints.DescriptionMaxLength);
        }
    }
}