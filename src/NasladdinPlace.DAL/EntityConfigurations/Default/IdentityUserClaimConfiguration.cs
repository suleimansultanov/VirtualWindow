using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class IdentityUserClaimConfiguration : BaseEntityConfiguration<IdentityUserClaim<int>>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<IdentityUserClaim<int>> builder)
        {
            builder.ToTable("UserClaims");
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<IdentityUserClaim<int>> builder)
        {
            builder.HasKey(iuc => iuc.Id);
        }
    }
}
