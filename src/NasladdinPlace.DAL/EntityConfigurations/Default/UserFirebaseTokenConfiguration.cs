using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class UserFirebaseTokenConfiguration : BaseEntityConfiguration<UserFirebaseToken>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<UserFirebaseToken> builder)
        {
            builder.Property(uft => uft.Token)
                .HasMaxLength(255)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<UserFirebaseToken> builder)
        {
            builder.HasKey(uft => uft.Id);

            builder
                .HasIndex(uft => new {uft.UserId, uft.Brand})
                .IsUnique();
        }
    }
}