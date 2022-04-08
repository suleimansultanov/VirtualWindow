using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models.Goods;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class GoodConfiguration : BaseEntityConfiguration<Good>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<Good> builder)
        {
            builder.Property(g => g.MakerId)
                .IsRequired();

            builder.Property(g => g.GoodCategoryId)
                .HasDefaultValueSql(GoodCategory.Default.Id.ToString())
                .IsRequired();

            builder.Property(g => g.Name)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(g => g.Description)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(g => g.PublishingStatus)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<Good> builder)
        {
            builder.Property(g => g.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<Good> builder)
        {
            builder.HasOne(g => g.Maker)
                .WithMany()
                .HasForeignKey(g => g.MakerId);

            builder.HasOne(g => g.GoodCategory)
                .WithMany(c => c.Goods)
                .HasForeignKey(g => g.GoodCategoryId);

            builder.HasOne(g => g.ProteinsFatsCarbohydratesCalories)
                .WithOne()
                .HasForeignKey<ProteinsFatsCarbohydratesCalories>(pfcc => pfcc.Id);
        }
    }
}
