using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class AppFeaturesToRoleConfiguration : BaseEntityConfiguration<AppFeatureItemsToRole>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<AppFeatureItemsToRole> builder)
        {
            builder.HasIndex(rpaf => rpaf.AppFeatureItemId);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<AppFeatureItemsToRole> builder)
        {
            builder.HasKey(rpaf => new { rpaf.RoleId, AppFeatureRecordId = rpaf.AppFeatureItemId });
        }
    }
}
