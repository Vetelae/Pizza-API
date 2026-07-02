using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizza_API.Constants;
using Pizza_API.Entities;

namespace Pizza_API.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(rt => rt.Token)
                .IsUnique();

            builder.Property(rt => rt.Token)
            .HasMaxLength(AuthConstraints.RefreshTokenMaxLength)
            .IsRequired();

            builder.Property(rt => rt.UserId)
                .IsRequired();
        }
    }
}