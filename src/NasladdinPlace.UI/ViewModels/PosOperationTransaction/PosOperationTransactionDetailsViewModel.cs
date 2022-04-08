using System.Collections.Generic;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Models.Fiscalization;

namespace NasladdinPlace.UI.ViewModels.PosOperationTransaction
{
    public class PosOperationTransactionDetailsViewModel : PosOperationTransactionViewModel
    {
        public ICollection<BankTransactionInfoVersionTwo> BankTransactionInfos { get; set; }
        public ICollection<FiscalizationInfoVersionTwo> FiscalizationInfos { get; set; }
        public BankTransactionInfoVersionTwo LastBankTransactionInfo { get; set; }
        public FiscalizationInfoVersionTwo LastFiscalizationInfo { get; set; }
    }
}
