using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class CityConfiguration : BaseEntityConfiguration<City>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<City> builder)
        {
            builder.Property(c => c.CountryId)
                .IsRequired();

            builder.Property(c => c.Name)
                .HasMaxLength(255)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<City> builder)
        {
            builder.Property(c => c.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<City> builder)
        {
            builder.HasOne(c => c.Country)
                .WithMany(c => c.Cities)
                .HasForeignKey(c => c.CountryId);
        }
    }
}
