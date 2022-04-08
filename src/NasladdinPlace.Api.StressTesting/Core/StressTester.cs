using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NasladdinPlace.Api.Client.Rest.Api;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Contracts;
using NasladdinPlace.Api.Client.Rest.AuthTokenManagement.Models;
using NasladdinPlace.Api.Client.Rest.Client.Contracts;
using NasladdinPlace.Api.Client.Rest.Client.Factory;
using NasladdinPlace.Api.Client.Rest.Dtos.Account;
using NasladdinPlace.Api.StressTesting.Core.Metrics;
using NasladdinPlace.Api.StressTesting.Core.UseCase.Contracts;
using NasladdinPlace.Api.StressTesting.Core.UseCase.Purchase;
using NasladdinPlace.Api.StressTesting.Models;

namespace NasladdinPlace.Api.StressTesting.Core
{
    public class StressTester : IStressTester
    {
        private readonly StressTestingConfig _config;
        private readonly IRestClient _restClient;
        private readonly IAuthTokenManager _authTokenManager;

        private readonly ICollection<IStressTestingUseCase> _stressTestingUseCases;

        public StressTester(StressTestingConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            _config = config;

            _authTokenManager = new InMemoryAuthTokenManager();
            _restClient = RestClientFactory.Create(config.BaseApiUrl, _authTokenManager);
            _stressTestingUseCases = new Collection<IStressTestingUseCase>
            {
                new PurchaseStressTestingUseCase(config.ConcurrentRequestsNumber, _restClient)
            };
        }
        
        public async Task<StressTestingReport> RunAsync()
        {
            var stressTestingUseCasesMetrics = new Collection<IReadOnlyStressTestingMetrics>();

            if (!await TryPerformAuthorization()) 
                return new StressTestingReport(stressTestingUseCasesMetrics);
            
            foreach (var stressTestingUseCase in _stressTestingUseCases)
            {
                var metrics = await stressTestingUseCase.StressTestAsync();
                stressTestingUseCasesMetrics.Add(metrics);
            }

            return new StressTestingReport(stressTestingUseCasesMetrics);
        }

        private async Task<bool> TryPerformAuthorization()
        {
            var userCredentials = _config.UsersCredentials.First();

            var authRequestResponse = await _restClient.PerformRequestAsync((IAuthApi api) => api.LoginUserAsync(
                new LoginDto
                {
                    UserName = userCredentials.UserName,
                    Password = userCredentials.Password
                }));

            if (!authRequestResponse.IsRequestSuccessful) 
                return false;
            
            var authRequestResult = authRequestResponse.Result;
            var authToken = new AuthToken(authRequestResult.Token, TimeSpan.FromSeconds(authRequestResult.ExpiresIn));
            await _authTokenManager.UpdateAsync(authToken);
            
            return true;

        }
    }
}