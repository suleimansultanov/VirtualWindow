using MailKit.Net.Smtp;
using MimeKit;
using NasladdinPlace.Api.Services.EmailSender.Contracts;
using NasladdinPlace.Api.Services.EmailSender.Models;
using Serilog;
using System;
using System.Threading.Tasks;

namespace NasladdinPlace.Api.Services.EmailSender
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpServerInfo _smtpServerInfo;
        private readonly ILogger _logger;

        public EmailSender(SmtpServerInfo smtpServerInfo, ILogger logger)
        {
            _smtpServerInfo = smtpServerInfo;
            _logger = logger;
        }

        public void Send(TextEmailMessage textEmailMessage)
        {
            try
            {
                SendMessage(CreateMessage(textEmailMessage));
            }
            catch (Exception ex)
            {
                _logger.Error($"Dispatch of the email to {textEmailMessage.DestinationEmail} has failed " +
                              $"because ${ex}.");
            }
        }

        public void SendAsync(TextEmailMessage textEmailMessage)
        {
            Task.Run(() => Send(textEmailMessage));
        }

        private MimeMessage CreateMessage(TextEmailMessage textEmailMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpServerInfo.MailboxCredentials.Email));
            message.To.Add(new MailboxAddress(textEmailMessage.DestinationEmail));
            message.Subject = textEmailMessage.Subject;

            var bodyBuilder = textEmailMessage.IsHtml
                ? new BodyBuilder { HtmlBody = textEmailMessage.Body }
                : new BodyBuilder { TextBody = textEmailMessage.Body };
            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }

        private void SendMessage(MimeMessage message)
        {
            using (var client = new SmtpClient())
            {
                client.Connect(
                    _smtpServerInfo.ConnectionInfo.Url,
                    _smtpServerInfo.ConnectionInfo.Port);

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(
                    _smtpServerInfo.MailboxCredentials.Email,
                    _smtpServerInfo.MailboxCredentials.Password);

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}