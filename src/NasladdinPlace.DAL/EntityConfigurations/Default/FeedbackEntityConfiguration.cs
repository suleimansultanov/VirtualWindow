using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.DAL.Entities;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class FeedbackEntityConfiguration : BaseEntityConfiguration<FeedbackEntity>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<FeedbackEntity> builder)
        {
            builder.Property(p => p.PhoneNumber)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Content)
                .HasMaxLength(3000)
                .IsRequired();
            
            builder.Property(p => p.DeviceName)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(p => p.DeviceOperatingSystem)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(p => p.AppVersion)
                .HasMaxLength(50)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<FeedbackEntity> builder)
        {
            builder.HasKey(p => p.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<FeedbackEntity> builder)
        {
            builder.HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId);

            builder.HasOne(f => f.Pos)
                .WithMany()
                .HasForeignKey(f => f.PosId);
        }
    }
}