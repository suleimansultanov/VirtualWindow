using System.Threading.Tasks;

namespace NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Contracts
{
    public interface IAuthTokenRemover
    {
        Task RemoveAuthTokenAsync();
    }
}