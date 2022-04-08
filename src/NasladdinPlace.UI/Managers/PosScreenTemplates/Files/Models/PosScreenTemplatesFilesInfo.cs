using System;
using System.Collections.Generic;

namespace NasladdinPlace.UI.Managers.PosScreenTemplates.Files.Models
{
    public class PosScreenTemplatesFilesInfo
    {
        public string WebRootPath { get; }
        public string FilesCommonDirectoryName { get; }
        public string TemplateDirectoryNameFormat { get; }
        public IEnumerable<string> RequiredFilesList { get; }

        public PosScreenTemplatesFilesInfo(
            string webRootPath,
            string filesCommonDirectoryName,
            string templateDirectoryNameFormat,
            IEnumerable<string> requiredFilesList)
        {
            if (string.IsNullOrEmpty(webRootPath))
                throw new ArgumentNullException(webRootPath);
            if (string.IsNullOrEmpty(templateDirectoryNameFormat))
                throw new ArgumentNullException(templateDirectoryNameFormat);
            if (string.IsNullOrEmpty(filesCommonDirectoryName))
                throw new ArgumentNullException(filesCommonDirectoryName);
            if (requiredFilesList == null)
                throw new ArgumentNullException(nameof(requiredFilesList));

            WebRootPath = webRootPath;
            FilesCommonDirectoryName = filesCommonDirectoryName;
            TemplateDirectoryNameFormat = templateDirectoryNameFormat;
            RequiredFilesList = requiredFilesList;
        }
    }
}