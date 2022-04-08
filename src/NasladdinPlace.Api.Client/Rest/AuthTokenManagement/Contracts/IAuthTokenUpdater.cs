using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Models;

namespace NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Contracts
{
    public interface IAuthTokenUpdater
    {
        Task UpdateAsync(AuthToken authToken);
    }
}