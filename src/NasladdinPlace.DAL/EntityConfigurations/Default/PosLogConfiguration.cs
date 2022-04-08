using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class PosLogConfiguration : BaseEntityConfiguration<PosLog>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<PosLog> builder)
        {
            builder.Property(pi => pi.FileName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(pi => pi.FileContent)
                .HasMaxLength(25 * 1024 * 1024) //25 mb
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<PosLog> builder)
        {
            builder.HasKey(ub => ub.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<PosLog> builder)
        {
            builder.HasOne(ub => ub.Pos)
                .WithMany()
                .HasForeignKey(ub => ub.PosId);
        }
    }
}
