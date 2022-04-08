using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class BankTransactionInfoVersionTwoConfiguration : BaseEntityConfiguration<BankTransactionInfoVersionTwo>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<BankTransactionInfoVersionTwo> builder)
        {
            builder.Property(bti => bti.Comment)
                .HasMaxLength(3000);

            builder.HasIndex(bti => bti.PosOperationTransactionId)
                .IsUnique(false);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<BankTransactionInfoVersionTwo> builder)
        {
            //intention left empty
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<BankTransactionInfoVersionTwo> builder)
        {
            builder.HasOne(bti => bti.PaymentCard)
                .WithMany()
                .HasForeignKey(bti => bti.PaymentCardId);

            builder.HasOne(bti => bti.PosOperationTransaction)
                .WithMany(pot => pot.BankTransactionInfos)
                .HasForeignKey(bti => bti.PosOperationTransactionId);
        }
    }
}
