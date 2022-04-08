using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders
{
    public class CachedPosTokenProviderByPosIdProvider : IPosTokenProviderByPosIdProvider
    {
        private readonly IPosTokenProviderByPosIdProvider _posTokenProviderByPosIdProvider;
        private readonly TimeSpan _cachePeriod;

        private readonly ConcurrentDictionary<int, PosTokenProviderWithLastProvisioningDate>
            _posTokenProviderByPosIdDictionary;

        public CachedPosTokenProviderByPosIdProvider(
            IPosTokenProviderByPosIdProvider posTokenProviderByPosIdProvider,
            TimeSpan cachePeriod)
        {
            if (posTokenProviderByPosIdProvider == null)
                throw new ArgumentNullException(nameof(posTokenProviderByPosIdProvider));
            
            _posTokenProviderByPosIdProvider = posTokenProviderByPosIdProvider;
            _cachePeriod = cachePeriod;
            _posTokenProviderByPosIdDictionary = 
                new ConcurrentDictionary<int, PosTokenProviderWithLastProvisioningDate>();
        }
        
        public async Task<IPosTokenProvider> ProvideAsync(int posId)
        {
            IPosTokenProvider posTokenProvider = null;
            if (_posTokenProviderByPosIdDictionary.TryGetValue(posId, out var posTokenProviderWithLastProvisioningDate) &&
                !posTokenProviderWithLastProvisioningDate.IsProvisioningDateExpired(expirationPeriod: _cachePeriod))
            {
                posTokenProvider = posTokenProviderWithLastProvisioningDate.PosTokenProvider;
            }

            if (posTokenProvider != null) 
                return posTokenProvider;
            
            posTokenProvider = await _posTokenProviderByPosIdProvider.ProvideAsync(posId);
            _posTokenProviderByPosIdDictionary[posId] =
                new PosTokenProviderWithLastProvisioningDate(posTokenProvider);

            return posTokenProvider;
        }
    }
}