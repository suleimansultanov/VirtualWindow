using System;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement
{
    public class StatelessPosTokenWithPrefixManager : IStatelessPosTokenManager
    {
        private readonly IStatelessPosTokenManager _statelessPosTokenManager;
        private readonly string _prefix;

        public StatelessPosTokenWithPrefixManager(IStatelessPosTokenManager statelessPosTokenManager, string prefix)
        {
            if (statelessPosTokenManager == null)
                throw new ArgumentNullException(nameof(statelessPosTokenManager));
            if (string.IsNullOrWhiteSpace(prefix))
                throw new ArgumentNullException(nameof(prefix));

            _statelessPosTokenManager = statelessPosTokenManager;
            _prefix = prefix;
        }
        
        public string GeneratePosToken(int posId)
        {
            var token = _statelessPosTokenManager.GeneratePosToken(posId);
            return $"{_prefix}{token}";
        }

        public bool IsPosTokenValid(int posId, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));
            
            return TryGetPosIdIfTokenValid(token, out var tokenPosId) && tokenPosId == posId;
        }

        public bool TryGetPosIdIfTokenValid(string token, out int posId)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));
            
            var tokenWithoutPrefix = token.Replace(_prefix, "");
            return _statelessPosTokenManager.TryGetPosIdIfTokenValid(tokenWithoutPrefix, out posId);
        }
    }
}