using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models.Goods;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class GoodCategoryConfiguration : BaseEntityConfiguration<GoodCategory>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<GoodCategory> builder)
        {
            builder.Property(gc => gc.Id)
                .ValueGeneratedOnAdd();

            builder.Property(gc => gc.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(gc => gc.ImagePath)
                .HasMaxLength(500);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<GoodCategory> builder)
        {
            builder.Property(gc => gc.Id)
                .IsRequired();
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<GoodCategory> builder)
        {
            builder.HasMany(gc => gc.Goods)
                .WithOne(gc => gc.GoodCategory)
                .HasForeignKey(g => g.GoodCategoryId);
        }
    }
}