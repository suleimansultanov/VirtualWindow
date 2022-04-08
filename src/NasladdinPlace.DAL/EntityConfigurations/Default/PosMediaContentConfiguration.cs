using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PosMediaContentConfiguration : BaseEntityConfiguration<PosMediaContent>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PosMediaContent> builder)
        {
            builder.HasKey(dm => new { dm.PosId, dm.MediaContentId });
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PosMediaContent> builder)
        {
            builder.HasOne(p => p.MediaContent)
                .WithMany()
                .HasForeignKey(p => p.MediaContentId);

            builder.HasOne(p => p.Pos)
                .WithMany()
                .HasForeignKey(p => p.PosId);
        }
    }
}
