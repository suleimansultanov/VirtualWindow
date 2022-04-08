using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PosScreenTemplateConfiguration : BaseEntityConfiguration<PosScreenTemplate>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PosScreenTemplate> builder)
        {
            builder.Property(pi => pi.Name)
                .HasMaxLength(255)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PosScreenTemplate> builder)
        {
            builder.HasKey(pt => pt.Id);
        }
    }
}