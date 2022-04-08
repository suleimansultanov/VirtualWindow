using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class LabeledGoodTrackingRecordConfiguration : BaseEntityConfiguration<LabeledGoodTrackingRecord>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<LabeledGoodTrackingRecord> builder)
        {
            // intentionally left empty
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<LabeledGoodTrackingRecord> builder)
        {
            builder.HasKey(r => r.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<LabeledGoodTrackingRecord> builder)
        {
            builder.HasOne(r => r.LabeledGood)
                .WithMany(lg => lg.TrackingRecords)
                .HasForeignKey(r => r.LabeledGoodId);

            builder.HasOne(r => r.Pos)
                .WithMany()
                .HasForeignKey(r => r.PosId);
        }
    }
}