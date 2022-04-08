using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class LabeledGoodConfiguration : BaseEntityConfiguration<LabeledGood>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<LabeledGood> builder)
        {
            builder.Property(lg => lg.Label)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(lg => lg.GoodId)
                .IsRequired(false);

            builder.HasIndex(lg => lg.Label)
                .IsUnique();

            builder.Property(e => e.Price)
                .IsRequired(false);
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<LabeledGood> builder)
        {
            builder.HasKey(lg => lg.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<LabeledGood> builder)
        {
            builder.HasOne(lg => lg.Pos)
                .WithMany(p => p.LabeledGoods)
                .HasForeignKey(lg => lg.PosId);

            builder.HasOne(lg => lg.PosOperation)
                .WithMany(upt => upt.LabeledGoods)
                .HasForeignKey(lg => new { lg.PosOperationId, lg.PosId });

            builder.HasOne(lg => lg.Good)
                .WithMany(g => g.LabeledGoods)
                .HasForeignKey(lg => lg.GoodId);

            builder.HasOne(d => d.Currency)
                   .WithMany()
                   .HasForeignKey(d => d.CurrencyId);
        }
    }
}
