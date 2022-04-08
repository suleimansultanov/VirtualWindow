namespace NasladdinPlace.DAL.EntityConfigurations.Contracts
{
    public interface IEntityConfigurationsFactory
    {
        IEntityConfigurations MakeEntityConfigurations();
    }
}