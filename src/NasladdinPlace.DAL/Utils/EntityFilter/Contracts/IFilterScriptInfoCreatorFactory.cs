namespace NasladdinPlace.DAL.Utils.EntityFilter.Contracts
{
    public interface IFilterScriptInfoCreatorFactory
    {
        IFilterScriptInfoCreator Create(EntityFilterContext context);
    }
}