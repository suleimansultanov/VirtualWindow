using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PosImageConfiguration : BaseEntityConfiguration<PosImage>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PosImage> builder)
        {
            builder.Property(pi => pi.ImagePath)
                .HasMaxLength(500)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PosImage> builder)
        {
            builder.HasKey(pi => pi.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<PosImage> builder)
        {
            builder.HasOne(pi => pi.Pos)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PosId);
        }
    }
}