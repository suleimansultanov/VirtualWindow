using NasladdinPlace.Api.Services.EmailSender.Models;

namespace NasladdinPlace.Api.Services.EmailSender.Contracts
{
    public interface IEmailSender
    {
        void Send(TextEmailMessage textEmailMessage);
        void SendAsync(TextEmailMessage textEmailMessage);
    }
}