using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class BankTransactionInfoConfiguration : BaseEntityConfiguration<BankTransactionInfo>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<BankTransactionInfo> builder)
        {
            builder.Property(bti => bti.Comment)
                .HasMaxLength(3000);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<BankTransactionInfo> builder)
        {
            //intention left empty
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<BankTransactionInfo> builder)
        {
            builder.HasOne(bti => bti.PaymentCard)
                .WithMany()
                .HasForeignKey(bti => bti.PaymentCardId);
        }
    }
}