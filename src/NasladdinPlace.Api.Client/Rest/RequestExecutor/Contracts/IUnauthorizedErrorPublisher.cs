using System;

namespace NasladdinPlace.Api.Client.Rest.RequestExecutor.Contracts
{
    public interface IUnauthorizedErrorPublisher
    {
        event EventHandler OnUnauthorizedError;
    }
}