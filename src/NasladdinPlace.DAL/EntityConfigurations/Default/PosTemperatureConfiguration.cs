using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PosTemperatureConfiguration : BaseEntityConfiguration<PosTemperature>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PosTemperature> builder)
        {
            // intentionally left empty. No properties to configure
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PosTemperature> builder)
        {
            builder.Property(g => g.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<PosTemperature> builder)
        {
            builder.HasOne(p => p.Pos)
                .WithMany()
                .HasForeignKey(p => p.PosId);
        }
    }
}