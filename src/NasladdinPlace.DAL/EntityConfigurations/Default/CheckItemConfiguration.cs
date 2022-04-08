using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class CheckItemConfiguration : BaseEntityConfiguration<CheckItem>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<CheckItem> builder)
        {
            builder.Property(e => e.Price)
                .IsRequired();

            builder.Property(e => e.Status)
                .IsRequired();

            builder.Property(po => po.DiscountAmount)
                .IsRequired()
                .HasDefaultValue(0M);

            builder.Ignore(c => c.RoundedDiscountAmount);

            builder.Ignore(c => c.PriceWithDiscount);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<CheckItem> builder)
        {
            builder.HasKey(c => c.Id);
        }
        
        protected override void ConfigureForeignKeys(EntityTypeBuilder<CheckItem> builder)
        {
            builder.HasOne(d => d.Currency)
                .WithMany()
                .HasForeignKey(d => d.CurrencyId);

            builder.HasOne(d => d.Good)
                .WithMany(g => g.CheckItems)
                .HasForeignKey(d => d.GoodId);

            builder.HasOne(d => d.LabeledGood)
                .WithMany()
                .HasForeignKey(d => d.LabeledGoodId);

            builder.HasOne(p => p.Pos)
                .WithMany()
                .HasForeignKey(ci => ci.PosId);
        }
    }
}
