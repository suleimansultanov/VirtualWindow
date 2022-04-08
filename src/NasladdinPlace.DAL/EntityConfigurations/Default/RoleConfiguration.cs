using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class RoleConfiguration : BaseEntityConfiguration<Role>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasMany(r => r.InternalPermittedAppFeatures)
                .WithOne()
                .HasForeignKey(rpaf => rpaf.RoleId);

            builder.HasMany(r=>r.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
        }
    }
}
