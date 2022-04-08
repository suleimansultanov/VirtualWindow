using System;
using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders
{
    public class SwitchablePosTokenProvider : IPosTokenProvider
    {
        private readonly IPosTokenProviderByPosIdProvider _posTokenProviderByPosIdProvider;

        public SwitchablePosTokenProvider(IPosTokenProviderByPosIdProvider posTokenProviderByPosIdProvider)
        {
            if (posTokenProviderByPosIdProvider == null)
                throw new ArgumentNullException(nameof(posTokenProviderByPosIdProvider));

            _posTokenProviderByPosIdProvider = posTokenProviderByPosIdProvider;
        }

        public async Task<ValueResult<string>> TryProvidePosTokenAsync(int posId)
        {
            var posTokenProvider = await _posTokenProviderByPosIdProvider.ProvideAsync(posId);
            return await posTokenProvider.TryProvidePosTokenAsync(posId);
        }
    }
}