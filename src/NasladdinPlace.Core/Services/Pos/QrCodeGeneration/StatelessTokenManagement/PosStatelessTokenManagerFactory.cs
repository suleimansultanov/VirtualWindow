using System;
using NasladdinPlace.Core.Services.Pos.QrCodeGeneration.Factory;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement
{
    public static class PosStatelessTokenManagerFactory
    {
        public static IStatelessPosTokenManager Create(PosTokenServicesOptions posTokenServicesOptions)
        {
            if (posTokenServicesOptions == null)
                throw new ArgumentNullException(nameof(posTokenServicesOptions));
            
            var tokenManager = new StatelessTokenManager(posTokenServicesOptions.TokenValidityPeriod);
            var cipher = new RijndaelCipher();
            var secureTokenManager = new SecureStatelessTokenManager(tokenManager, cipher, posTokenServicesOptions.EncryptionKey);
            var simplePosTokenManager = new StatelessPosTokenManager(secureTokenManager);
            var escapedPosTokenManager = new EscapedStatelessPosTokenManager(simplePosTokenManager);
            return posTokenServicesOptions.HasTokenPrefix
                    ? (IStatelessPosTokenManager) new StatelessPosTokenWithPrefixManager(escapedPosTokenManager, posTokenServicesOptions.TokenPrefix)
                    : simplePosTokenManager;
        }
    }
}