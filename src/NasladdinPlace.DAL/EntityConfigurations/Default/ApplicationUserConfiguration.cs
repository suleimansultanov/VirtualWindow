using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class ApplicationUserConfiguration : BaseEntityConfiguration<ApplicationUser>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("Users");
            
            builder.Property(u => u.FullName)
                .HasMaxLength(255);

            builder.Property(u => u.ChangePhoneNumberTokenRemainder)
                .HasMaxLength(10);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(u => u.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasOne(u => u.ActivePaymentCard)
                .WithMany()
                .HasForeignKey(u => u.ActivePaymentCardId);

            builder.HasMany(u => u.FirebaseTokens)
                .WithOne()
                .HasForeignKey(uft => uft.UserId);

            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        }
    }
}
