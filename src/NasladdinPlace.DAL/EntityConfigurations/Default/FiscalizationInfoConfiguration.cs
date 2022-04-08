using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Fiscalization;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class FiscalizationInfoConfiguration : BaseEntityConfiguration<FiscalizationInfo>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<FiscalizationInfo> builder)
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

            builder.Property(fi => fi.State)
                .IsConcurrencyToken();
            
            builder.Property(fi => fi.QrCodeValue)
                .HasMaxLength(150);

            builder.HasIndex(fi => fi.RequestId)
                .IsUnique();
        }
        
        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<FiscalizationInfo> builder)
        {
            builder.HasKey(fi => fi.Id);
        }
    }
}
