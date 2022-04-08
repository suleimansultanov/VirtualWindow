using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class DocumentGoodsMovingTableItemConfiguration : BaseEntityConfiguration<DocumentGoodsMovingTableItem>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<DocumentGoodsMovingTableItem> builder)
        {
            //intentionally left empty
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<DocumentGoodsMovingTableItem> builder)
        {
            builder.Property(p => p.Id).IsRequired();
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<DocumentGoodsMovingTableItem> builder)
        {
            builder.HasOne(dti => dti.Document)
                .WithMany(idoc => idoc.TablePart)
                .HasForeignKey(dti => dti.DocumentId);
        }
    }
}
