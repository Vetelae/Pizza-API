using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizza_API.Constants;
using Pizza_API.Entities;

namespace Pizza_API.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            builder.Property(o => o.CustomerName)
                .HasMaxLength(OrderConstraints.NameMaxLength)
                .IsRequired();

            builder.Property(o => o.CustomerEmail)
            .HasMaxLength(OrderConstraints.EmailMaxLength)
            .IsRequired();

            builder.Property(o => o.CustomerPhone)
                .HasMaxLength(OrderConstraints.PhoneMaxLength)
                .IsRequired();

            builder.Property(o => o.DeliveryAddress)
                .HasMaxLength(OrderConstraints.AddressMaxLength);

            builder.Property(o => o.Notes)
                .HasMaxLength(OrderConstraints.NotesMaxLength);
        }
    }
}