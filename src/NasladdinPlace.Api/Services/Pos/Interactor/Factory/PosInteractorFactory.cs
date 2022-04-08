using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Pos.Interactor;

namespace NasladdinPlace.Api.Services.Pos.Interactor.Factory
{
    public class PosInteractorFactory : IPosInteractorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PosInteractorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPosInteractor Create()
        {
            return _serviceProvider.GetService<IPosInteractor>();
        }
    }
}
