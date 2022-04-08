using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class MessengerContactConfiguration : BaseEntityConfiguration<MessengerContact>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<MessengerContact> builder)
        {
            builder.HasIndex(e => e.Type)
                .IsUnique();

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(100)
                .IsRequired();
        }

        protected override void ConfigurePrimaryKeys(EntityTypeBuilder<MessengerContact> builder)
        {
            builder.HasKey(sp => sp.Id);
        }
    }
}