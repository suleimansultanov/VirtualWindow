using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class IdentityRoleClaimConfiguration : BaseEntityConfiguration<IdentityRoleClaim<int>>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<IdentityRoleClaim<int>> builder)
        {
            builder.ToTable("RoleClaims");
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<IdentityRoleClaim<int>> builder)
        {
            builder.HasKey(irc => irc.Id);
        }
    }
}
