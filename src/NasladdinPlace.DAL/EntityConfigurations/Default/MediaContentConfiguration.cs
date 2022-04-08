using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class MediaContentConfiguration : BaseEntityConfiguration<MediaContent>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<MediaContent> builder)
        {
            builder.Property(pi => pi.FileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(pi => pi.FileContent)
                .HasMaxLength(25 * 1024 * 1024) //25 mb
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<MediaContent> builder)
        {
            builder.HasKey(ub => ub.Id);
        }
    }
}
