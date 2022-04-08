namespace NasladdinPlace.UI.ViewModels.Checks
{
    public class CheckSummaryViewModel
    {
        private long _refundSum;
        private long _additionOrVerificationSum;

        public long RefundSum {
            get => GetCorrectedSum(_refundSum);
            set => _refundSum = value;
        }

        public long AdditionOrVerificationSum {
            get => GetCorrectedSum(_additionOrVerificationSum);
            set => _additionOrVerificationSum = value;
        }

        private static long GetCorrectedSum(long sum)
        {
            return sum / 100;
        }
    }
}
