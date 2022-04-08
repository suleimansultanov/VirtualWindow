using NasladdinPlace.Core.Services.Formatters.Contracts;

namespace NasladdinPlace.Core.Services.Formatters
{
    public class TelegramLinkWrapper : ILinkWrapper
    {
        private readonly ILinkTypeFormatProvider _linkTypeFormatProvider;

        public TelegramLinkWrapper(ILinkTypeFormatProvider linkTypeFormatProvider)
        {
            _linkTypeFormatProvider = linkTypeFormatProvider;
        }

        public string Wrap(string content, LinkFormatType type, params object[] formattingArgs)
        {
            return $"[{content}]" +
                   $"({string.Format(_linkTypeFormatProvider.GetFormat(type), formattingArgs)})";
        }
    }
}
