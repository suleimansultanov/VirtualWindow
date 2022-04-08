using System;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement
{
    public class EscapedStatelessPosTokenManager : IStatelessPosTokenManager
    {
        private readonly IStatelessPosTokenManager _statelessPosTokenManager;
        
        public EscapedStatelessPosTokenManager(IStatelessPosTokenManager statelessPosTokenManager)
        {
            if (statelessPosTokenManager == null)
                throw new ArgumentNullException(nameof(statelessPosTokenManager));
            
            _statelessPosTokenManager = statelessPosTokenManager;
        }
        
        public string GeneratePosToken(int posId)
        {
            var token = _statelessPosTokenManager.GeneratePosToken(posId);
            return EscapeToken(token);
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
            
            var unescapedToken = UnescapeToken(token);
            return _statelessPosTokenManager.TryGetPosIdIfTokenValid(unescapedToken, out posId);
        }

        private static string EscapeToken(string token)
        {
            return token.Replace('=', '#')
                .Replace('/', '@')
                .Replace('+', '~');
        }

        private static string UnescapeToken(string token)
        {
            return token.Replace('#', '=')
                .Replace('@', '/')
                .Replace('~', '+');
        }
    }
}