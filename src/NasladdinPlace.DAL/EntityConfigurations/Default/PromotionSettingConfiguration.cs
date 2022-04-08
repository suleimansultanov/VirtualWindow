using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PromotionSettingConfiguration : BaseEntityConfiguration<PromotionSetting>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PromotionSetting> builder)
        {
            builder.HasIndex(lg => lg.PromotionType)
                   .IsUnique();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PromotionSetting> builder)
        {
            builder.HasKey(ub => ub.Id);
        }
    }
}
