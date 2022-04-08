using System.Diagnostics.Contracts;

namespace NasladdinPlace.Core.Models.Feedback
{
    public class SenderInfo
    {
        public static SenderInfo UnauthorizedSenderInfo(
            string phoneNumber,
            DeviceInfo deviceInfo)
        {
            return new SenderInfo(null, phoneNumber, deviceInfo);
        }

        public static SenderInfo AuthorizedSenderInfoWithLastPurchase(
            ApplicationUser user,
            DeviceInfo deviceInfo,
            LastPurchaseInfo lastPurchaseInfo,
            PaymentBalance paymentBalance)
        {
            return new SenderInfo(user, user.PhoneNumber ?? string.Empty, deviceInfo, paymentBalance, lastPurchaseInfo);
        }

        public static SenderInfo AuthorizedSenderInfoWithoutLastPurchase(
            ApplicationUser user,
            DeviceInfo deviceInfo,
            PaymentBalance paymentBalance)
        {
            return new SenderInfo(user, user.PhoneNumber ?? string.Empty, deviceInfo, paymentBalance);
        }
        
        public ApplicationUser User { get; }
        public LastPurchaseInfo LastPurchaseInfo { get; }
        public string PhoneNumber { get; }
        public DeviceInfo DeviceInfo { get; }
        public PaymentBalance PaymentBalance { get; }

        public SenderInfoStatus Status
        {
            get
            {
                if (IsSenderUnauthorized)
                {
                    return SenderInfoStatus.Unauthorized;
                }
                if (!HasLastPurchaseInfo)
                {
                    return SenderInfoStatus.NoPosOperations;
                }

                return SenderInfoStatus.HasLastPurchase;
            }
        }

        private SenderInfo(
            ApplicationUser user,
            string phoneNumber,
            DeviceInfo deviceInfo,
            PaymentBalance paymentBalance = null,
            LastPurchaseInfo lastPurchaseInfo = null)
        {
            Contract.Assert(deviceInfo != null);
            Contract.Assert(user != null || phoneNumber != null);
            DeviceInfo = deviceInfo;
            User = user;
            PhoneNumber = phoneNumber;
            PaymentBalance = paymentBalance;
            LastPurchaseInfo = lastPurchaseInfo;
        }

        public bool IsSenderUnauthorized => User == null;

        public bool HasLastPurchaseInfo => LastPurchaseInfo != null;
    }
}