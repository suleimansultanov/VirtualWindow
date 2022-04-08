namespace NasladdinPlace.Core.Services.Formatters.Contracts
{
    public interface ILinkWrapper
    {
        string Wrap(string content, LinkFormatType type, params object[] formattingArgs);
    }
}