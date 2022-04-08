using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class AppFeatureItemConfiguration : BaseEntityConfiguration<AppFeatureItem>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<AppFeatureItem> builder)
        {
            builder.Property(appFeature => appFeature.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(appFeature => appFeature.Description)
                .HasMaxLength(255)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<AppFeatureItem> builder)
        {
            builder.HasKey(appFeature => appFeature.Id);
        }
    }
}
