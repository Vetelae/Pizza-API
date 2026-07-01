using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pizza_API.Constants;
using Pizza_API.Entities;

namespace Pizza_API.Configurations
{
    public class NewsConfiguration : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.Property(n => n.Title)
                .HasMaxLength(NewsConstraints.TitleMaxLength)
                .IsRequired();

            builder.Property(n => n.Content)
                .HasMaxLength(NewsConstraints.ContentMaxLength)
                .IsRequired();
        }
    }
}