using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models.Fiscalization;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class FiscalizationInfoVersionTwoConfiguration : BaseEntityConfiguration<FiscalizationInfoVersionTwo>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<FiscalizationInfoVersionTwo> builder)
        {
            builder.Property(fi => fi.ErrorInfo)
                .HasMaxLength(3000);

            builder.Property(fi => fi.DocumentInfo)
                .HasMaxLength(3000);

            builder.Property(fi => fi.FiscalizationNumber)
                .HasMaxLength(50);

            builder.Property(fi => fi.FiscalizationSerial)
                .HasMaxLength(50);

            builder.Property(fi => fi.FiscalizationSign)
                .HasMaxLength(50);

            builder.Property(fi => fi.QrCodeValue)
                .HasMaxLength(150);

            builder.Property(fi => fi.OfdCheckUrl)
                .HasMaxLength(250);

            builder.Property(fi => fi.RequestId)
                .HasMaxLength(50);

            builder.Property(fi => fi.State)
                .IsConcurrencyToken();

            builder.HasIndex(fi => fi.PosOperationTransactionId)
                .IsUnique(false);

            builder.HasIndex(fi => new { fi.RequestId, fi.RequestDateTime })
                .IsUnique();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<FiscalizationInfoVersionTwo> builder)
        {
            builder.HasKey(fi => fi.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<FiscalizationInfoVersionTwo> builder)
        {
            builder.HasOne(fi => fi.PosOperationTransaction)
                .WithMany(pot => pot.FiscalizationInfos)
                .HasForeignKey(fi => fi.PosOperationTransactionId);
        }
    }
}
