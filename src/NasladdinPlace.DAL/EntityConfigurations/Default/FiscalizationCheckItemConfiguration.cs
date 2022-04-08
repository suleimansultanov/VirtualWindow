using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class FiscalizationCheckItemConfiguration : BaseEntityConfiguration<FiscalizationCheckItem>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<FiscalizationCheckItem> builder)
        {
            builder.Property(f => f.CheckItemId)
                .IsRequired();

            builder.Property(f => f.FiscalizationInfoId)
                .IsRequired();

            builder.Property(f => f.Amount)
                .HasDefaultValue(0.0)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<FiscalizationCheckItem> builder)
        {
            builder.HasKey(f => f.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<FiscalizationCheckItem> builder)
        {
            builder.HasOne(cki => cki.CheckItem)
                .WithMany(f => f.FiscalizationCheckItems)
                .HasForeignKey(d => d.CheckItemId);

            builder.HasOne(d => d.FiscalizationInfo)
                .WithMany(p => p.FiscalizationCheckItems)
                .HasForeignKey(d => d.FiscalizationInfoId);
        }
    }
}