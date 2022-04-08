namespace NasladdinPlace.Api.Services.EmailSender.Models
{
    public class TextEmailMessage
    {
        public string DestinationEmail { get; }
        public string Subject { get; }
        public string Body { get; }
        public bool IsHtml { get; set; }

        public TextEmailMessage(string destinationEmail, string subject, string body, bool isHtml = false)
        {
            DestinationEmail = destinationEmail;
            Subject = subject;
            Body = body;
            IsHtml = isHtml;
        }
    }
}