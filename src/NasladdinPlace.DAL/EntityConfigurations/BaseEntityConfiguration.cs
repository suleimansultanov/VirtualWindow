using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NasladdinPlace.DAL.EntityConfigurations.Contracts;

namespace NasladdinPlace.DAL.EntityConfigurations
{
    public abstract class BaseEntityConfiguration<T> : IEntityConfiguration<T> where T : class
    {
        public Action<EntityTypeBuilder<T>> ProvideApplyAction()
        {
            return builder =>
            {
                ConfigureProperties(builder);
                ConfigurePrimaryKeys(builder);
                ConfigureForeignKeys(builder);
            };
        }

        protected abstract void ConfigureProperties(EntityTypeBuilder<T> builder);
        protected abstract void ConfigurePrimaryKeys(EntityTypeBuilder<T> builder);
        protected virtual void ConfigureForeignKeys(EntityTypeBuilder<T> builder) { }
    }
}