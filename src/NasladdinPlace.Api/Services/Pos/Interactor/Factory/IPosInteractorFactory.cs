using NasladdinPlace.Core.Services.Pos.Interactor;

namespace NasladdinPlace.Api.Services.Pos.Interactor.Factory
{
    public interface IPosInteractorFactory
    {
        IPosInteractor Create();
    }
}
