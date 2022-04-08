using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class UserNotificationConfiguration : BaseEntityConfiguration<UserNotification>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<UserNotification> builder)
        {
            builder.Property(un => un.MessageText)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<UserNotification> builder)
        {
            builder.HasKey(un => un.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<UserNotification> builder)
        {
            builder.HasOne(un => un.User)
                .WithMany()
                .HasForeignKey(un => un.UserId);
        }
    }
}
