namespace NasladdinPlace.Core.Services.Check.Refund.Models
{
    public interface ICheckManagerResult
    {
        bool IsSuccessful { get; }
        string Error { get; }
    }
}
