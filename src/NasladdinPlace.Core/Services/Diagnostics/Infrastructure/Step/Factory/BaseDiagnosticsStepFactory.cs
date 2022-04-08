using System;
using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Models;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Step.Factory
{
    public abstract class BaseDiagnosticsStepFactory : IDiagnosticsStepFactory
    {
        private readonly IServiceScope _scope;

        protected BaseDiagnosticsStepFactory(IServiceScope serviceScope)
        {
            if (serviceScope == null)
                throw new ArgumentNullException(nameof(serviceScope));

            _scope = serviceScope;
        }
        
        protected T GetRequiredService<T>()
        {
            return _scope.ServiceProvider.GetRequiredService<T>();
        }

        public abstract DiagnosticsStep Create();
    }
}