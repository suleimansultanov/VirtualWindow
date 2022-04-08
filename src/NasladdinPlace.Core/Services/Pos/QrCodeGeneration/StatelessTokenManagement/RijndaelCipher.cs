using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NasladdinPlace.Core.Services.Pos.QrCodeGeneration.StatelessTokenManagement
{
    public class RijndaelCipher : ICipher
    {
        private const int BlockSize = 128;
        private const int KeySizeInBytes = BlockSize / 8;
        private const int DerivationIterations = 1000;
        private const CipherMode CipherMode = System.Security.Cryptography.CipherMode.CBC;
        private const PaddingMode PaddingMode = System.Security.Cryptography.PaddingMode.PKCS7;

        public string Encrypt(string plainText, string passPhrase)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrWhiteSpace(passPhrase))
                throw new ArgumentNullException(nameof(passPhrase));
            
            EnsureParametersValid(plainText, passPhrase);
            
            var saltStringBytes = GenerateBitsOfRandomEntropy();
            var ivStringBytes = GenerateBitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(KeySizeInBytes);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = BlockSize;
                    symmetricKey.Mode = CipherMode;
                    symmetricKey.Padding = PaddingMode;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public string Decrypt(string cipherText, string passPhrase)
        {
            EnsureParametersValid(cipherText, passPhrase);
            
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            var saltStringBytes = cipherTextBytesWithSaltAndIv
                .Take(KeySizeInBytes)
                .ToArray();
            var ivStringBytes = cipherTextBytesWithSaltAndIv
                .Skip(KeySizeInBytes)
                .Take(KeySizeInBytes)
                .ToArray();
            var cipherTextBytes = cipherTextBytesWithSaltAndIv
                .Skip(KeySizeInBytes * 2)
                .Take(cipherTextBytesWithSaltAndIv.Length - KeySizeInBytes * 2)
                .ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(KeySizeInBytes);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = BlockSize;
                    symmetricKey.Mode = CipherMode;
                    symmetricKey.Padding = PaddingMode;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private void EnsureParametersValid(string plainText, string passPhrase)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                throw new ArgumentNullException(nameof(plainText));
            if (string.IsNullOrWhiteSpace(passPhrase)) 
                throw new ArgumentNullException(nameof(passPhrase));
        }

        private static byte[] GenerateBitsOfRandomEntropy()
        {
            var randomBytes = new byte[BlockSize / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}