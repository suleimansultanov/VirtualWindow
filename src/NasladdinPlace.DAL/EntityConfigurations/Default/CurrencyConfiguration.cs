using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class CurrencyConfiguration : BaseEntityConfiguration<Currency>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<Currency> builder)
        {
            builder.Property(p => p.Name)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(p => p.IsoCode)
                .HasMaxLength(3)
                .HasDefaultValue("RUB")
                .IsRequired();

            builder.HasAlternateKey(p => p.IsoCode);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<Currency> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }
}
