using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NasladdinPlace.DAL.EntityConfigurations.Contracts
{
    public interface IEntityConfiguration<T> where T : class
    {
        Action<EntityTypeBuilder<T>> ProvideApplyAction();
    }
}