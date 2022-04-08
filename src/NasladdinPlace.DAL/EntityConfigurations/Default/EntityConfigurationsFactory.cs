using NasladdinPlace.DAL.EntityConfigurations.Contracts;

namespace NasladdinPlace.DAL.EntityConfigurations.Default
{
    public class EntityConfigurationsFactory : IEntityConfigurationsFactory
    {
        public IEntityConfigurations MakeEntityConfigurations()
        {
            return new EntityConfigurations();
        }
    }
}