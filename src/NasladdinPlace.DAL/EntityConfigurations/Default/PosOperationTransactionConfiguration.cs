using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PosOperationTransactionConfiguration : BaseEntityConfiguration<PosOperationTransaction>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PosOperationTransaction> builder)
        {
            builder.Property(pot => pot.PosId)
                .IsRequired();

            builder.Property(pot => pot.BonusAmount)
                .HasDefaultValue(0M);

            builder.Property(pot => pot.MoneyAmount)
                .HasDefaultValue(0M);

            builder.Property(pot => pot.FiscalizationAmount)
                .HasDefaultValue(0M);

            builder.Property(po => po.Status).IsConcurrencyToken();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PosOperationTransaction> builder)
        {
            builder.HasKey(pot => pot.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<PosOperationTransaction> builder)
        {

            builder.HasOne(pot => pot.LastBankTransactionInfo)
                .WithMany()
                .HasForeignKey(pot => pot.LastBankTransactionInfoId);

            builder.HasOne(pot => pot.LastFiscalizationInfo)
                .WithMany()
                .HasForeignKey(pot => pot.LastFiscalizationInfoId);

            builder.HasMany(pot => pot.BonusPoints)
                .WithOne(bp => bp.PosOperationTransaction)
                .HasForeignKey(bp => bp.PosOperationTransactionId);

            builder.HasMany(pot => pot.PosOperationTransactionCheckItems)
                .WithOne(pci => pci.PosOperationTransaction)
                .HasForeignKey(pci => pci.PosOperationTransactionId);
        }
    }
}
