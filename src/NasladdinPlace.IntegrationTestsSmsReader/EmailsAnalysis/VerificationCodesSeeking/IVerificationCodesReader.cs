using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.IntegrationTestsSmsReader.Common;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodesSeeking
{
    public interface IVerificationCodesReader
    {
        Task<ValueResult<IEnumerable<VerificationCode>>> ReadAsync();
    }
}