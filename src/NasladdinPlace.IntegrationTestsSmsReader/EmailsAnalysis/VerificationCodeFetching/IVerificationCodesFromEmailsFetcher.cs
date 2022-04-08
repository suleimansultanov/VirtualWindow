using System.Collections.Generic;
using NasladdinPlace.IntegrationTestsSmsReader.Common;
using NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.Models;

namespace NasladdinPlace.IntegrationTestsSmsReader.EmailsAnalysis.VerificationCodeFetching
{
    public interface IVerificationCodesFromEmailsFetcher
    {
        IEnumerable<EmailVerificationCode> Fetch(IEnumerable<Email> emails);
        bool TryFetchCode(Email email, out EmailVerificationCode emailVerificationCode);
    }
}