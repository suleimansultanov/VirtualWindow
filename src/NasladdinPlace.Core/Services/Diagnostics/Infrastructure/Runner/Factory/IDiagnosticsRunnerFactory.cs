using Microsoft.Extensions.DependencyInjection;

namespace NasladdinPlace.Core.Services.Diagnostics.Infrastructure.Runner.Factory
{
    public interface IDiagnosticsRunnerFactory
    {
        IDiagnosticsRunner Create(IServiceScope serviceScope);
    }
}