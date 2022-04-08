using System;
using System.IO;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace NasladdinPlace.Api.Services.Logs.User
{
    public class FolderAbsolutePathProvider : IFolderAbsolutePathProvider
    {
        private readonly IHostingEnvironment _environment;

        public FolderAbsolutePathProvider(IHostingEnvironment environment)
        {
            if (environment == null)
                throw new ArgumentNullException(nameof(environment));
            
            _environment = environment;
        }
        
        public string Provide(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentNullException(nameof(folderName));
            
            return Path.Combine(_environment.WebRootPath, folderName);
        }
    }
}