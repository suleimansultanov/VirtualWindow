using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PaymentCardConfiguration : BaseEntityConfiguration<PaymentCard>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PaymentCard> builder)
        {
            builder.Property(pk => pk.Token)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(pc => pc.FirstSixDigits)
                .HasMaxLength(6);

            builder.Property(pc => pc.LastFourDigits)
                .HasMaxLength(4);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PaymentCard> builder)
        {
            builder.HasKey(pk => pk.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<PaymentCard> builder)
        {
            builder.HasOne(pk => pk.User)
                .WithMany(u => u.PaymentCards)
                .HasForeignKey(pk => pk.UserId);
        }
    }
}