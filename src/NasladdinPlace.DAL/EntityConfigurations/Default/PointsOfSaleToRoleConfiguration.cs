using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PointsOfSaleToRoleConfiguration : BaseEntityConfiguration<PointsOfSaleToRole>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PointsOfSaleToRole> builder)
        {
            builder.HasIndex(patr => patr.PosId);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PointsOfSaleToRole> builder)
        {
            builder.HasKey(patr => new { patr.RoleId, patr.PosId });
        }
    }
}
