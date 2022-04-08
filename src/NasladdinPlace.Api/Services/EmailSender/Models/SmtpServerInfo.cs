namespace NasladdinPlace.Api.Services.EmailSender.Models
{
    public sealed class SmtpServerInfo
    {
        public ConnectionInfo ConnectionInfo { get; }
        public MailboxCredentials MailboxCredentials { get; }

        public SmtpServerInfo(ConnectionInfo connectionInfo, MailboxCredentials mailboxCredentials)
        {
            ConnectionInfo = connectionInfo;
            MailboxCredentials = mailboxCredentials;
        }
    }
}