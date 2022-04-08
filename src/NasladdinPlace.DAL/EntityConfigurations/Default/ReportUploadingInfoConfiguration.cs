using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class ReportUploadingInfoConfiguration : BaseEntityConfiguration<ReportUploadingInfo>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<ReportUploadingInfo> builder)
        {
            builder.Property(e => e.Url)
                .HasMaxLength(255)
                .IsRequired();

            builder.HasIndex(e => e.Type)
                .IsUnique();

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            builder.Property(e => e.Sheet)
                .HasMaxLength(100)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<ReportUploadingInfo> builder)
        {
            builder.HasKey(sp => sp.Id);
        }
    }
}
