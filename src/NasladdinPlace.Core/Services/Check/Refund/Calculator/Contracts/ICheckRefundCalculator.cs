using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.Check.Refund.Calculator.Models;
using System.Collections.Generic;

namespace NasladdinPlace.Core.Services.Check.Refund.Calculator.Contracts
{
    public interface ICheckRefundCalculator
    {
        RefundCalculationResult Calculate(PosOperation operation, IEnumerable<CheckItem> checkItems);
    }
}
