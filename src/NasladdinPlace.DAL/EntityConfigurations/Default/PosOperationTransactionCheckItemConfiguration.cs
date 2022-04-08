using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PosOperationTransactionCheckItemConfiguration: BaseEntityConfiguration<PosOperationTransactionCheckItem>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PosOperationTransactionCheckItem> builder)
        {
            builder.Property(pci => pci.CheckItemId)
                .IsRequired();

            builder.Property(pci => pci.PosOperationTransactionId)
                .IsRequired();

            builder.Property(pci => pci.CreationDate)
                .IsRequired();

            builder.Property(pci => pci.Amount)
                .HasDefaultValue(0M)
                .IsRequired();

            builder.HasIndex(pci => pci.CheckItemId)
                .IsUnique(false);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PosOperationTransactionCheckItem> builder)
        {
            builder.HasKey(p => p.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<PosOperationTransactionCheckItem> builder)
        {
            builder.HasOne(pci => pci.CheckItem)
                .WithMany(cki => cki.PosOperationTransactionCheckItems)
                .HasForeignKey(cki => cki.CheckItemId);

            builder.HasOne(pot => pot.PosOperationTransaction)
                .WithMany(pci => pci.PosOperationTransactionCheckItems)
                .HasForeignKey(pti => pti.PosOperationTransactionId);
        }
    }
}
