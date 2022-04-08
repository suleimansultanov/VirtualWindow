using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PosDoorsStateConfiguration : BaseEntityConfiguration<PosDoorsState>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PosDoorsState> builder)
        {    
             // no need to configure properties
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PosDoorsState> builder)
        {
            builder.Property(g => g.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<PosDoorsState> builder)
        {
            builder.HasOne(p => p.Pos)
                .WithMany()
                .HasForeignKey(p => p.PosId);

            builder.HasOne(p => p.PosOperation)
                .WithMany(po => po.PosDoorsStates)
                .HasForeignKey(p => new { p.PosOperationId, p.PosId });
        }
    }
}