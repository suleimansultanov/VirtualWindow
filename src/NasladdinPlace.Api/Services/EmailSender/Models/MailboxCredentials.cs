namespace NasladdinPlace.Api.Services.EmailSender.Models
{
    public sealed class MailboxCredentials
    {
        public string Email { get; }
        public string Password { get; }

        public MailboxCredentials(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}