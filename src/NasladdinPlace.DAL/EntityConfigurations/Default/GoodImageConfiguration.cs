using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models.Goods;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class GoodImageConfiguration : BaseEntityConfiguration<GoodImage>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<GoodImage> builder)
        {
            builder.Property(gi => gi.ImagePath)
                .HasMaxLength(500)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<GoodImage> builder)
        {
            builder.HasKey(gi => gi.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<GoodImage> builder)
        {
            builder.HasOne(gi => gi.Good)
                .WithMany(g => g.GoodImages);
        }
    }
}
