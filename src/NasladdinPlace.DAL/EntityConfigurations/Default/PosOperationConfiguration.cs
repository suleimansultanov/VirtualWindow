using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PosOperationConfiguration : BaseEntityConfiguration<PosOperation>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PosOperation> builder)
        {
            builder.Property(po => po.Id)
                .ValueGeneratedOnAdd();

            builder.Property(po => po.UserId)
                .IsRequired();

            builder.Property(po => po.PosId)
                .IsRequired();

            builder.Property(po => po.DateStarted)
                .IsRequired();

            builder.Property(po => po.BonusAmount)
                .IsRequired()
                .HasDefaultValue(0M);

            builder.Property(po => po.Mode)
                .IsRequired();

            builder.Property(po => po.Status).IsConcurrencyToken();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PosOperation> builder)
        {
            builder.HasKey(po => new { po.Id, po.PosId });
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<PosOperation> builder)
        {
            builder.HasOne(po => po.User)
                .WithMany(u => u.PosOperations)
                .HasForeignKey(upt => upt.UserId);

            builder.HasOne(po => po.Pos)
                .WithMany(p => p.Operations)
                .HasForeignKey(upt => upt.PosId);

            builder.HasMany(po => po.BankTransactionInfos)
                .WithOne()
                .HasForeignKey(bti => new { bti.PosOperationId, bti.PosId });

            builder.HasMany(po => po.CheckItems)
                .WithOne(ci => ci.PosOperation)
                .HasForeignKey(ci => new {ci.PosOperationId, ci.PosId});

            builder.HasMany(po => po.FiscalizationInfos)
                .WithOne(f => f.PosOperation)
                .HasForeignKey(ci => new { ci.PosOperationId, ci.PosId});

            builder.HasMany(po => po.PosOperationTransactions)
                .WithOne(pot => pot.PosOperation)
                .HasForeignKey(pot => new { pot.PosOperationId, pot.PosId });

            builder.HasMany(po => po.DocumentsGoodsMoving)
                .WithOne(inv => inv.PosOperation)
                .HasForeignKey(inv => new { inv.PosOperationId, inv.PosId });
        }
    }
}
