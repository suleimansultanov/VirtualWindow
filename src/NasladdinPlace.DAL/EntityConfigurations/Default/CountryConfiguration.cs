using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class CountryConfiguration : BaseEntityConfiguration<Country>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<Country> builder)
        {
            builder.Property(p => p.Name)
                .HasMaxLength(255)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<Country> builder)
        {
            builder.Property(p => p.Id).IsRequired();
        }
    }
}
