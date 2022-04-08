using System.Collections.Generic;

namespace NasladdinPlace.Core.Services.TextFileReader
{
    public interface ITextFileReader
    {
        IEnumerable<string> ReadFile(string relativePath);
    }
}