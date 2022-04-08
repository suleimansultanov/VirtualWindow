namespace NasladdinPlace.Core.Services.Feedback.Printer
{
    public interface IFeedbackPrinter
    {
        string Print(Models.Feedback.Feedback feedback);
    }
}