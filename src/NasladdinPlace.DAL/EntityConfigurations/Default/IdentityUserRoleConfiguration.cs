using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class IdentityUserRoleConfiguration : BaseEntityConfiguration<UserRole>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(iur => new { iur.RoleId, iur.UserId });
        }
    }
}
