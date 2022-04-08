using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class IdentityUserLoginConfiguration : BaseEntityConfiguration<IdentityUserLogin<int>>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<IdentityUserLogin<int>> builder)
        {
            builder.ToTable("UserLogins");
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<IdentityUserLogin<int>> builder)
        {
            builder.HasKey(iul => new {iul.ProviderKey, iul.LoginProvider});
        }
    }
}
