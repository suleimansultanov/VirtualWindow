using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models.Discounts;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class DiscountConfiguration : BaseEntityConfiguration<Discount>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<Discount> builder)
        {
            builder.Property(m => m.Name)
                .HasMaxLength(500)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<Discount> builder)
        {
            builder.HasKey(ub => ub.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<Discount> builder)
        {
            builder.HasMany(dr => dr.DiscountRules)
                   .WithOne(d => d.Discount)
                   .HasForeignKey(d => d.DiscountId );

            builder.HasMany(dr => dr.PosDiscounts)
                   .WithOne(d => d.Discount)
                   .HasForeignKey(d => d.DiscountId);
        }
    }
}
