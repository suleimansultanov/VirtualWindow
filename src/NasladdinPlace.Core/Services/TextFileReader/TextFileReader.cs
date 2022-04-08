using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NasladdinPlace.Core.Services.TextFileReader
{
    public class TextFileReader : ITextFileReader
    {
        private readonly string _webRootPath;

        public TextFileReader(string webRootPath)
        {
            _webRootPath = webRootPath;
        }
        
        public IEnumerable<string> ReadFile(string relativePath)
        {
            try
            {
                var absolutePath = CreateAbsolutePath(relativePath);
                return File.ReadAllLines(absolutePath);
            }
            catch (Exception)
            {
                return Enumerable.Empty<string>();
            }
        }
        
        private string CreateAbsolutePath(string relatievePath)
        {
            return Path.Combine(_webRootPath, relatievePath);
        }
    }
}