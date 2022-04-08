using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models.Discounts;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class DiscountRuleConfiguration : BaseEntityConfiguration<DiscountRule>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<DiscountRule> builder)
        {
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<DiscountRule> builder)
        {
            builder.HasKey(ub => ub.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<DiscountRule> builder)
        {
            builder.HasMany(dr => dr.DiscountRuleValues)
                .WithOne(d => d.DiscountRule)
                .HasForeignKey(d => d.DiscountRuleId);
        }
    }
}
