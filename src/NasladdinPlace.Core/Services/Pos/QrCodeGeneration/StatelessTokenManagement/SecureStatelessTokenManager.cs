using System;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement
{
    public class SecureStatelessTokenManager : IStatelessTokenManager
    {
        private readonly IStatelessTokenManager _statelessTokenManager;
        private readonly ICipher _cipher;
        private readonly string _encryptionPhrase;

        public SecureStatelessTokenManager(IStatelessTokenManager statelessTokenManager, ICipher cipher, string encryptionPhrase)
        {
            if (statelessTokenManager == null)
                throw new ArgumentNullException(nameof(statelessTokenManager));
            if (cipher == null)
                throw new ArgumentNullException(nameof(cipher));
            if (string.IsNullOrWhiteSpace(encryptionPhrase))
                throw new ArgumentNullException(nameof(encryptionPhrase));
            
            _statelessTokenManager = statelessTokenManager;
            _cipher = cipher;
            _encryptionPhrase = encryptionPhrase;
            _encryptionPhrase = encryptionPhrase;
        }
        
        public string GenerateToken(byte[] tokenData)
        {
            if (tokenData == null)
                throw new ArgumentNullException(nameof(tokenData));

            var token = _statelessTokenManager.GenerateToken(tokenData);

            return _cipher.Encrypt(token, _encryptionPhrase);
        }

        public bool IsValidToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            var decryptedToken = DecryptToken(token);

            return _statelessTokenManager.IsValidToken(decryptedToken);
        }

        public byte[] GetTokenData(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            var decryptedToken = DecryptToken(token);

            return _statelessTokenManager.GetTokenData(decryptedToken);
        }

        private string DecryptToken(string token)
        {
            return _cipher.Decrypt(token, _encryptionPhrase);
        }
    }
}