using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Services.MessageSender.Sms.Models;

namespace NasladdinPlace.Core.Services.MessageSender.Sms.Contracts
{
    public interface ISmsSender
    {
        event EventHandler<decimal> BalanceAlmostExceededHandler;
        event EventHandler<SmsLoggingInfo> SmsServiceErrorHandler;
        Task<bool> SendSmsAsync(SmsLoggingInfo smsInfo);
    }
}
