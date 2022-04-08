using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace NasladdinPlace.Api.Services.Identity.Security
{
    public static class MutableRfc6238AuthenticationService
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly Encoding Encoding = new UTF8Encoding(false, true);
        private static readonly TimeSpan TimeStep = TimeSpan.FromSeconds(60);

        internal static int ComputeTotp(
          HashAlgorithm hashAlgorithm,
          ulong timeStepNumber,
          string modifier)
        {
            const int mod = 1000000;

            var bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long)timeStepNumber));
            var hash = hashAlgorithm.ComputeHash(ApplyModifier(bytes, modifier));
            var offset = hash[hash.Length - 1] & 15;

            var binaryCode = (hash[offset] & 0x7f) << 24
                             | (hash[offset + 1] & 0xff) << 16
                             | (hash[offset + 2] & 0xff) << 8
                             | (hash[offset + 3] & 0xff);

            return binaryCode % mod;
        }

        private static byte[] ApplyModifier(byte[] input, string modifier)
        {
            if (string.IsNullOrEmpty(modifier))
                return input;

            var bytes = Encoding.GetBytes(modifier);
            var numArray = new byte[checked(input.Length + bytes.Length)];
            Buffer.BlockCopy(input, 0, numArray, 0, input.Length);
            Buffer.BlockCopy(bytes, 0, numArray, input.Length, bytes.Length);

            return numArray;
        }

        private static ulong GetCurrentTimeStepNumber()
        {
            return (ulong)((DateTime.UtcNow - UnixEpoch).Ticks / TimeStep.Ticks);
        }

        public static int GenerateCode(byte[] securityToken, string modifier = null)
        {
            if (securityToken == null)
                throw new ArgumentNullException(nameof(securityToken));

            var currentTimeStepNumber = GetCurrentTimeStepNumber();

            using (var hashAlgorithm = new HMACSHA1(securityToken))
                return ComputeTotp(hashAlgorithm, currentTimeStepNumber, modifier);
        }

        public static bool ValidateCode(byte[] securityToken, int code, string modifier = null)
        {
            if (securityToken == null)
                throw new ArgumentNullException(nameof(securityToken));

            var currentTimeStepNumber = GetCurrentTimeStepNumber();
            using (var hashAlgorithm = new HMACSHA1(securityToken))
            {
                for (var index = -2; index <= 2; ++index)
                {
                    if (ComputeTotp(hashAlgorithm, currentTimeStepNumber + (ulong)index, modifier) == code)
                        return true;
                }
            }

            return false;
        }
    }
}