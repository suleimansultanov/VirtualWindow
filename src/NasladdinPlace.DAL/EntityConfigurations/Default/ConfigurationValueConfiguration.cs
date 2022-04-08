using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class ConfigurationValueConfiguration : BaseEntityConfiguration<ConfigurationValue>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<ConfigurationValue> builder)
        {
            builder.Property(v => v.Value)
                .HasMaxLength(1000)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<ConfigurationValue> builder)
        {
            builder.HasKey(cv => cv.Id);
        }
    }
}