using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosByTokenProviders
{
    public class SwitchablePosByTokenProvider : IPosByTokenProvider
    {
        private readonly IEnumerable<IPosByTokenProvider> _posByTokenProviders;

        public SwitchablePosByTokenProvider(IEnumerable<IPosByTokenProvider> posByTokenProviders)
        {
            if (posByTokenProviders == null)
                throw new ArgumentNullException(nameof(posByTokenProviders));
            
            _posByTokenProviders = posByTokenProviders;
        }
        
        public Task<ValueResult<Models.Pos>> TryProvideByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            return TryProvideByTokenAuxAsync(token);
        }

        private async Task<ValueResult<Models.Pos>> TryProvideByTokenAuxAsync(string token)
        {
            foreach (var posByTokenProvider in _posByTokenProviders)
            {
                var posResult = await posByTokenProvider.TryProvideByTokenAsync(token);

                if (posResult.Succeeded) return posResult;
            }

            return ValueResult<Models.Pos>.Failure("Token is incorrect or pos does not exist.");
        }
    }
}