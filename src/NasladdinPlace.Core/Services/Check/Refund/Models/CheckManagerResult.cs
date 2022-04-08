using System;
using NasladdinPlace.Core.Services.Check.Helpers;

namespace NasladdinPlace.Core.Services.Check.Refund.Models
{
    public class CheckManagerResult : ICheckManagerResult
    {
        public static CheckManagerResult Success(CheckEditingInfo checkEditingInfo)
        {
            return new CheckManagerResult(true, string.Empty, checkEditingInfo);
        }

        public static CheckManagerResult Success()
        {
            return new CheckManagerResult(true, string.Empty);
        }

        public static CheckManagerResult NeedToCompletePurchase(CheckEditingInfo checkEditingInfo)
        {
            return new CheckManagerResult(checkEditingInfo);
        }

        public static CheckManagerResult Failure(string error)
        {
            return new CheckManagerResult(false, error);
        }

        public static CheckManagerResult Failure(string error, CheckEditingInfo checkEditingInfo)
        {
            return new CheckManagerResult(false, error, checkEditingInfo);
        }

        public bool IsSuccessful { get; }
        public bool ShouldPayForPurchase { get; }
        public bool IsPaidViaMoney => CheckEditingInfo?.MoneyAmount > 0;
        public string Error { get; }
        public CheckEditingInfo CheckEditingInfo { get; }

        private CheckManagerResult(bool isSuccessful, string error)
        {
            IsSuccessful = isSuccessful;
            Error = error;
        }
        private CheckManagerResult(CheckEditingInfo checkEditingInfo) : this(true, null, checkEditingInfo)
        {
            ShouldPayForPurchase = true;
        }

        private CheckManagerResult(bool isSuccessful, string error, CheckEditingInfo checkEditingInfo) : this(isSuccessful, error)
        {
            if (checkEditingInfo == null)
                 throw new ArgumentNullException(nameof(checkEditingInfo));

            CheckEditingInfo = checkEditingInfo;
        }

        public int GetUserId()
        {
            return CheckEditingInfo.PosOperation.UserId;
        }
    }
}
