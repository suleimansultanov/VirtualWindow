using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class MediaContentToPosPlatformConfiguration : BaseEntityConfiguration<MediaContentToPosPlatform>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<MediaContentToPosPlatform> builder)
        {
            builder.Property(po => po.Id)
                .ValueGeneratedOnAdd();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<MediaContentToPosPlatform> builder)
        {
            builder.HasKey(dm => new {dm.Id});
        }
        protected override void ConfigureForeignKeys(EntityTypeBuilder<MediaContentToPosPlatform> builder)
        {
            builder.HasOne(ub => ub.MediaContent)
                .WithMany()
                .HasForeignKey(ub => ub.MediaContentId);
        }
    }
}
