using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class IdentityUserTokenConfiguration: BaseEntityConfiguration<IdentityUserToken<int>>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<IdentityUserToken<int>> builder)
        {
            builder.ToTable("UserTokens");
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<IdentityUserToken<int>> builder)
        {
            builder.HasKey(iut => iut.UserId);
        }
    }
}
