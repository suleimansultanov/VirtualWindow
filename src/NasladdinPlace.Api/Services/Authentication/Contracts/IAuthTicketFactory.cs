using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using NasladdinPlace.Api.Services.Authentication.Models;

namespace NasladdinPlace.Api.Services.Authentication.Contracts
{
    public interface IAuthTicketFactory
    {
        Task<AuthenticationTicket> CreateAsync(TicketInitParams ticketInitParams);
    }
}