using System;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement
{
    public class StatelessPosTokenManager : IStatelessPosTokenManager
    {
        private const int InvalidPosId = -1;
        
        private readonly IStatelessTokenManager _statelessTokenManager;

        public StatelessPosTokenManager(IStatelessTokenManager statelessTokenManager)
        {
            if (statelessTokenManager == null)
                throw new ArgumentNullException(nameof(statelessTokenManager));
            
            _statelessTokenManager = statelessTokenManager;
        }

        public string GeneratePosToken(int posId)
        {
            var posIdAsBytes = BitConverter.GetBytes(posId);
            return _statelessTokenManager.GenerateToken(posIdAsBytes);
        }

        public bool IsPosTokenValid(int posId, string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            return TryGetPosIdIfTokenValid(token, out var tokenPosId) && posId == tokenPosId;
        }

        public bool TryGetPosIdIfTokenValid(string token, out int posId)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            posId = InvalidPosId;
            
            try
            {
                var tokenDataAsBytes = _statelessTokenManager.GetTokenData(token);
                posId = BitConverter.ToInt32(tokenDataAsBytes, 0);
                return _statelessTokenManager.IsValidToken(token);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}