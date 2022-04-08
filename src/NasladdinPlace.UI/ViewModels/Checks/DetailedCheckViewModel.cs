using System.Collections.Generic;
using NasladdinPlace.UI.Dtos.BankTransaction;
using NasladdinPlace.UI.Dtos.Check;
using NasladdinPlace.UI.Dtos.User;
using NasladdinPlace.UI.ViewModels.Fiscalization;
using NasladdinPlace.UI.ViewModels.PosOperationTransaction;

namespace NasladdinPlace.UI.ViewModels.Checks
{
    public class DetailedCheckViewModel
    {
        public UserDto User { get; set; }
        public ICollection<BankTransactionInfoDto> Transactions { get; set; }
        public DetailedCheckDto Check { get; set; }
        public List<FiscalizationInfoViewModel> FiscalizationChecks { get; set; }
        public ICollection<PosOperationTransactionViewModel> OperationTransactions { get; set; }
        public AuditDateTimeDto AuditDateTime { get; set; }
    }
}
