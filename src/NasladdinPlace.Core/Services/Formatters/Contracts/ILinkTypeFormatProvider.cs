namespace NasladdinPlace.Core.Services.Formatters.Contracts
{
    public interface ILinkTypeFormatProvider
    {
        string GetFormat(LinkFormatType type);
    }
}