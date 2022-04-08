using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class MakerConfiguration : BaseEntityConfiguration<Maker>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<Maker> builder)
        {
            builder.Property(m => m.Name)
                .HasMaxLength(1000)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<Maker> builder)
        {
            builder.Property(m => m.Id).IsRequired();
        }
    }
}
