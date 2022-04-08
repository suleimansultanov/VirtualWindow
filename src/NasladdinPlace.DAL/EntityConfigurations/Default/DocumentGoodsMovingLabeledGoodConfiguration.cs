using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class DocumentGoodsMovingLabeledGoodConfiguration : BaseEntityConfiguration<DocumentGoodsMovingLabeledGood>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<DocumentGoodsMovingLabeledGood> builder)
        {
            //intentionally left empty
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<DocumentGoodsMovingLabeledGood> builder)
        {
            builder.Property(p => p.Id).IsRequired();
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<DocumentGoodsMovingLabeledGood> builder)
        {
            builder.HasOne(g => g.DocumentGoodsMovingTableItem)
                .WithMany()
                .HasForeignKey(g => g.DocumentTableItemId);

        }
    }
}
