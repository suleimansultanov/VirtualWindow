using System;
using System.Linq;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement
{
    public class StatelessTokenManager : IStatelessTokenManager
    {
        private const int BytesInLong = 8;
        
        private readonly TimeSpan _tokenValidityPeriod;

        public StatelessTokenManager(TimeSpan tokenValidityPeriod)
        {
            _tokenValidityPeriod = tokenValidityPeriod;
        }
        
        public string GenerateToken(byte[] tokenData)
        {
            if (tokenData == null)
                throw new ArgumentNullException(nameof(tokenData));
            
            var tokenExpirationTime = DateTime.UtcNow.Add(_tokenValidityPeriod);
            var tokenExpirationTimeAsBytes = BitConverter.GetBytes(tokenExpirationTime.ToBinary());
            var tokenAsBytes = tokenExpirationTimeAsBytes.Concat(tokenData).ToArray();
            return Convert.ToBase64String(tokenAsBytes);
        }

        public bool IsValidToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));
            
            var tokenAsBytes = Convert.FromBase64String(token);
            var tokenExpirationDateAsBinary = BitConverter.ToInt64(tokenAsBytes, 0);
            var tokenExpirationDate = DateTime.FromBinary(tokenExpirationDateAsBinary);
            return tokenExpirationDate >= DateTime.UtcNow;
        }

        public byte[] GetTokenData(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));

            return Convert.FromBase64String(token).Skip(BytesInLong).ToArray();
        }
    }
}