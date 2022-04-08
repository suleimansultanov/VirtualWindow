using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PosAbnormalSensorMeasurementConfiguration : BaseEntityConfiguration<PosAbnormalSensorMeasurement>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PosAbnormalSensorMeasurement> builder)
        {
            builder.Property(ps => ps.MeasurementValue)
                .HasDefaultValue(0.0);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PosAbnormalSensorMeasurement> builder)
        {
            builder.HasKey(ps => ps.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<PosAbnormalSensorMeasurement> builder)
        {
            builder.HasOne(ps => ps.Pos)
                .WithMany(p => p.PosAbnormalSensorMeasurements)
                .HasForeignKey(pf => pf.PosId);
        }
    }
}