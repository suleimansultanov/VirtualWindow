using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class CheckItemAuditRecordConfiguration : BaseEntityConfiguration<CheckItemAuditRecord>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<CheckItemAuditRecord> builder)
        {
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<CheckItemAuditRecord> builder)
        {
            builder.HasKey(ar => ar.Id);
        }

        protected override void ConfigureForeignKeys(EntityTypeBuilder<CheckItemAuditRecord> builder)
        {
            builder.HasOne(ar => ar.CheckItem)
                .WithMany(cki => cki.AuditRecords)
                .HasForeignKey(ar => ar.CheckItemId);

            builder.HasOne(ar => ar.User)
                .WithMany(u => u.CheckItemsAuditHistory)
                .HasForeignKey(ar => ar.EditorId);
        }
    }
}