using NasladdinPlace.Core;

namespace NasladdinPlace.UI.Managers.Reference.UniReferencesManagers
{
    public class BaseUniReferencesManager
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected BaseUniReferencesManager(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}
