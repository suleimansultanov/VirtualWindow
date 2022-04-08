using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class UserBonusPointConfiguration : BaseEntityConfiguration<UserBonusPoint>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<UserBonusPoint> builder)
        {
            // intentionally left empty
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<UserBonusPoint> builder)
        {
            builder.HasKey(ub => ub.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<UserBonusPoint> builder)
        {
            builder.HasOne(ub => ub.User)
                .WithMany()
                .HasForeignKey(ub => ub.UserId);

            builder.HasOne(ud => ud.User)
                .WithMany(p => p.BonusPoints)
                .HasForeignKey(ud => ud.UserId);
        }
    }
}