using Microsoft.Extensions.DependencyInjection;
using NasladdinPlace.Api.Services.EmailSender.Contracts;
using NasladdinPlace.Api.Services.EmailSender.Models;
using Serilog;

namespace NasladdinPlace.Api.Services.EmailSender.Extensions
{
    public static class EmailSenderExtensions
    {
        public static void AddEmailSender(
            this IServiceCollection serviceCollection, 
            SmtpServerInfo smtpServerInfo)
        {
            serviceCollection.AddSingleton<IEmailSender>(sp => new EmailSender(
                smtpServerInfo, sp.GetRequiredService<ILogger>())
            );
        }
    }
}