using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NasladdinPlace.Api.Services.EmailSender.Contracts;
using NasladdinPlace.Api.Services.EmailSender.Models;
using NasladdinPlace.Application.Services.Feedbacks;
using NasladdinPlace.Application.Services.Feedbacks.Contracts;
using NasladdinPlace.Core.Services.Feedback;
using NasladdinPlace.Core.Services.Feedback.Builder.SenderInfo;
using NasladdinPlace.Core.Services.Feedback.Printer;
using NasladdinPlace.Core.Services.MessageSender.Telegram.Contracts;

namespace NasladdinPlace.Api.Services.Feedback
{
    public static class FeedbackServiceExtensions
    {
        public static void AddFeedbackService(this IServiceCollection services)
        {
            services.AddSingleton<IFeedbackService, FeedbackService>();
            services.AddTransient<IFeedbackPrinter, FullFeedbackPrinter>();
            services.AddTransient<ISenderInfoFactory, SenderInfoFactory>();
            services.TryAddScoped<IFeedbackAppService, FeedbackAppService>();
        }

        public static void UseFeedbacksEmailNotifications(
            this IApplicationBuilder app,
            string subject,
            ICollection<string> destinationEmails)
        {
            var serviceProvider = app.ApplicationServices;
            
            var feedbackService = serviceProvider.GetRequiredService<IFeedbackService>();
            
            var feedbackPrinter = serviceProvider.GetRequiredService<IFeedbackPrinter>();

            feedbackService.OnFeedbackAdded += (sender, args) =>
            {
                var emailSender = serviceProvider.GetRequiredService<IEmailSender>();
                var emailMessages = destinationEmails
                    .Select(destinationEmail => new TextEmailMessage(
                        destinationEmail, subject, feedbackPrinter.Print(args.AddedFeedback)))
                    .ToImmutableList();
                
                emailMessages.ForEach(message =>
                {
                    emailSender.SendAsync(message);
                });
            };
        }

        public static void UseFeedbacksTelegramNotifications(
            this IApplicationBuilder app,
            long chatId)
        {
            var serviceProvider = app.ApplicationServices;
            
            var feedbackService = serviceProvider.GetRequiredService<IFeedbackService>();
            
            feedbackService.OnFeedbackAdded += (sender, args) =>
            {
                var telegramMessageSender = serviceProvider.GetRequiredService<IMessageSender>();
                var feedbackPrinter = serviceProvider.GetRequiredService<IFeedbackPrinter>();
                var messagesText = feedbackPrinter.Print(args.AddedFeedback);

                telegramMessageSender.SendMessage(chatId, messagesText);
            };
        }
    }
}