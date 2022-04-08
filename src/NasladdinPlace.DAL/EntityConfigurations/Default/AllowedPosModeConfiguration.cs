using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class AllowedPosModeConfiguration : BaseEntityConfiguration<AllowedPosMode>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<AllowedPosMode> builder)
        {
            // intentionally left empty
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<AllowedPosMode> builder)
        {
            builder.HasKey(apom => new {apom.PosId, apom.Mode});
        }
    }
}