using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizza_API.Constants;
using Pizza_API.Entities;

namespace Pizza_API.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.Property(ci => ci.UnitPrice)
                .HasPrecision(18, 2);

            builder.Property(ci => ci.Notes)
                .HasMaxLength(CartItemConstraints.NotesMaxLength);
        }
    }
}