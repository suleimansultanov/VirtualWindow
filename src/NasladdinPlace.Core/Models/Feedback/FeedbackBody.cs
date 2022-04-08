namespace NasladdinPlace.Core.Models.Feedback
{
    public class FeedbackBody
    {
        public string Content { get; }

        public FeedbackBody(string content)
        {
            Content = content;
        }
    }
}