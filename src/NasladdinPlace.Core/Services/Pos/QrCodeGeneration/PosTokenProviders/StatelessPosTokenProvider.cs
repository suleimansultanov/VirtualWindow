using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.PosTokenProviders
{
    public class StatelessPosTokenProvider : IPosTokenProvider
    {
        private readonly IStatelessPosTokenManager _statelessPosTokenManager;

        public StatelessPosTokenProvider(IStatelessPosTokenManager statelessPosTokenManager)
        {
            if (statelessPosTokenManager == null)
                throw new ArgumentNullException(nameof(statelessPosTokenManager));
            
            _statelessPosTokenManager = statelessPosTokenManager;
        }
        
        public Task<ValueResult<string>> TryProvidePosTokenAsync(int posId)
        {
            var posToken = _statelessPosTokenManager.GeneratePosToken(posId);
            return Task.FromResult(ValueResult<string>.Success(posToken));
        }
    }
}