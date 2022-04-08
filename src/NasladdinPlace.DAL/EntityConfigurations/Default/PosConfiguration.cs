using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PosConfiguration : BaseEntityConfiguration<Pos>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<Pos> builder)
        {
            builder.Property(p => p.CityId)
                .IsRequired();

            builder.Property(p => p.Longitude)
                .IsRequired();

            builder.Property(p => p.Latitude)
                .IsRequired();

            builder.Property(p => p.Street)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(p => p.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(p => p.AbbreviatedName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.QrCode)
                .HasMaxLength(100)
                .IsRequired()
                .HasDefaultValue("8081c0fc-a73e-4397-a282-8e0a2c351330");

            builder.Property(p => p.AreNotificationsEnabled)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Ignore(p => p.ScreenResolutionOrNull);

            builder.Property(p => p.UseNewPaymentSystem)
                .HasDefaultValue(false);

            builder.Property(p => p.IsRestrictedAccess)
                .HasDefaultValue(false);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<Pos> builder)
        {
            builder.HasKey(p => p.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<Pos> builder)
        {
            builder.HasOne(p => p.City)
                .WithMany()
                .HasForeignKey(p => p.CityId);

            builder.HasMany(p => p.InternalAllowedModes)
                .WithOne()
                .HasForeignKey(apom => apom.PosId);
        }
    }
}
