using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PromotionLogConfiguration : BaseEntityConfiguration<PromotionLog>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PromotionLog> builder)
        {
            // intentionally left empty
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PromotionLog> builder)
        {
            builder.HasKey(ub => ub.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<PromotionLog> builder)
        {
            builder.HasOne(ub => ub.User)
                .WithMany(u => u.PromotionLogs)
                .HasForeignKey(ub => ub.UserId);
        }
    }
}
