using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class RolePermittedAppFeatureConfiguration : BaseEntityConfiguration<RolePermittedAppFeature>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<RolePermittedAppFeature> builder)
        {
            builder.HasIndex(rpaf => rpaf.AppFeature);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<RolePermittedAppFeature> builder)
        {
            builder.HasKey(rpaf => new { rpaf.RoleId, rpaf.AppFeature });
        }
    }
}