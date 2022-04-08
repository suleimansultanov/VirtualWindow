using NasladdinPlace.Core.Services.Formatters.Contracts;
using System.Collections.Generic;

namespace NasladdinPlace.Core.Services.Formatters
{
    public class LinkTypeFormatProvider : ILinkTypeFormatProvider
    {
        private readonly Dictionary<string, string> _adminPageLinkFormats;

        public LinkTypeFormatProvider(Dictionary<string, string> adminPageLinkFormats)
        {
            _adminPageLinkFormats = adminPageLinkFormats;
        }

        public string GetFormat(LinkFormatType type)
        {
            return _adminPageLinkFormats[type.ToString()];
        }
    }
}
