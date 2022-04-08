namespace NasladdinPlace.Core.Services.Users.Test.Models
{
    public interface IReadOnlyTestUserInfo
    {
        int UserId { get; }
        string UserName { get; }
        bool IsPaymentCardVerificationRequired { get; }
    }
}