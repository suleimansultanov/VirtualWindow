using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models.Configuration;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class ConfigurationKeyConfiguration : BaseEntityConfiguration<ConfigurationKey>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<ConfigurationKey> builder)
        {
            builder.Property(ck => ck.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(ck => ck.Description)
                .HasMaxLength(1000);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<ConfigurationKey> builder)
        {
            builder.HasKey(ck => ck.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<ConfigurationKey> builder)
        {
            base.ConfigureForeignKeys(builder);

            builder.HasMany(ck => ck.Values)
                .WithOne()
                .HasForeignKey(v => v.KeyId);

            builder.HasMany(ck => ck.Children)
                .WithOne()
                .HasForeignKey(ck => ck.ParentId);
        }
    }
}