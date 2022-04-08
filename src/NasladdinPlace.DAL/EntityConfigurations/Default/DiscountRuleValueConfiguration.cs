using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models.Discounts;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class DiscountRuleValueConfiguration : BaseEntityConfiguration<DiscountRuleValue>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<DiscountRuleValue> builder)
        {
            builder.Property(m => m.Value)
                .HasMaxLength(250)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<DiscountRuleValue> builder)
        {
            builder.HasKey(ub => ub.Id);
        }
    }
}
