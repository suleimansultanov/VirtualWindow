using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Models;

namespace NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Contracts
{
    public interface IAuthTokenRetriever
    {
        Task<AuthToken> RetrieveAsync();
    }
}