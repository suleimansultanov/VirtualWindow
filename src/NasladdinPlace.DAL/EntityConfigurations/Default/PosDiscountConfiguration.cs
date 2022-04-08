using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models.Discounts;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PosDiscountConfiguration : BaseEntityConfiguration<PosDiscount>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PosDiscount> builder)
        {            
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PosDiscount> builder)
        {
            builder.Property(g => g.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<PosDiscount> builder)
        {
            builder.HasOne(p => p.Pos)
                .WithMany()
                .HasForeignKey(p => p.PosId);
        }
    }
}
