using System.Threading.Tasks;
using NasladdinPlace.IntegrationTestsSmsReader.Common;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodesSeeking
{
    public interface IVerificationCodeByTypeSeeker
    {
        Task<ValueResult<VerificationCode>> FindFirstAsync(VerificationCodeType verificationCodeType);
    }
}