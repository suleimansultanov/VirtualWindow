using NasladdinPlace.Core.Services.Purchase.Manager.Contracts;

namespace NasladdinPlace.Core.Services.Purchase.Factory
{
    public interface IPurchaseManagerFactory
    {
        IPurchaseManager Create();
    }
}