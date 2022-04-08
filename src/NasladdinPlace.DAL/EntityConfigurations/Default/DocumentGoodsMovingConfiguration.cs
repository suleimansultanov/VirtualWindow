using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class DocumentGoodsMovingConfiguration : BaseEntityConfiguration<DocumentGoodsMoving>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<DocumentGoodsMoving> builder)
        {
            //intentionally left empty
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<DocumentGoodsMoving> builder)
        {
            builder.Property(p => p.Id).IsRequired();
        }
    }
}
