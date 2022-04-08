using Microsoft.AspNetCore.Http;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Files.Contracts;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Files.Models;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Managers.PosScreenTemplates.Files
{
    public class PosScreenTemplateFilesManager : IPosScreenTemplateFilesManager
    {
        private readonly PosScreenTemplatesFilesInfo _posScreenTemplatesFilesInfo;

        public PosScreenTemplateFilesManager(PosScreenTemplatesFilesInfo posScreenTemplatesFilesInfo)
        {
            if (posScreenTemplatesFilesInfo == null)
                throw new ArgumentNullException(nameof(posScreenTemplatesFilesInfo));

            _posScreenTemplatesFilesInfo = posScreenTemplatesFilesInfo;
        }

        public async Task<IReadOnlyCollection<Result>> UploadTemplateFilesAsync(int templateId, IEnumerable<IFormFile> files)
        {
            var uploadFilesTasks = files.Select(async file => await UploadTemplateFileAsync(templateId, file)).ToList();

            var uploadTemplateFilesResultList = (await Task.WhenAll(uploadFilesTasks)).ToImmutableList();

            return uploadTemplateFilesResultList;
        }

        public async Task<Result> UploadTemplateFileAsync(int templateId, IFormFile file)
        {
            var templateFolderPath = GetTemplateDirectoryAbsolutePath(templateId);

            try
            {
                if (file.Length > 0)
                {
                    var filePath = Path.Combine(templateFolderPath, file.FileName);

                    var fileInfo = new FileInfo(filePath);
                    if (fileInfo.Exists)
                        return Result.Failure();

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }

            return Result.Success();
        }

        public IReadOnlyCollection<string> GetTemplateFilesNames(int templateId)
        {
            var directory = new DirectoryInfo(GetTemplateDirectoryAbsolutePath(templateId));

            return directory
                .GetFiles()
                .Select(f => f.Name)
                .ToImmutableList();
        }

        public async Task<Result> CreateTemplateDirectoryAsync(int templateId)
        {
            try
            {
                var templateFolderPath = GetTemplateDirectoryAbsolutePath(templateId);

                if (Directory.Exists(templateFolderPath))
                    return Result.Failure();

                await Task.Run(() => Directory.CreateDirectory(GetTemplateDirectoryAbsolutePath(templateId)));
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }

            return Result.Success();
        }

        public async Task<Result> DeleteTemplateDirectoryAsync(int templateId)
        {
            try
            {
                var templateFolderPath = GetTemplateDirectoryAbsolutePath(templateId);

                if (!Directory.Exists(templateFolderPath))
                    return Result.Failure();

                await Task.Run(() => Directory.Delete(templateFolderPath, true));
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }

            return Result.Success();
        }

        public async Task<Result> DeleteTemplateFileAsync(int templateId, string fileName)
        {
            try
            {
                var filePath = Path.Combine(GetTemplateDirectoryAbsolutePath(templateId), fileName);
                var fileInfo = new FileInfo(filePath);

                if (!fileInfo.Exists)
                    return Result.Failure();

                await Task.Run(() => fileInfo.Delete());             
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }

            return Result.Success();
        }

        public string GetTemplateDirectoryPath(int templateId)
        {
            return Path.Combine(
                _posScreenTemplatesFilesInfo.FilesCommonDirectoryName,
                string.Format(_posScreenTemplatesFilesInfo.TemplateDirectoryNameFormat, templateId));
        }

        public IReadOnlyCollection<string> GetMissingRequiredFileNamesForTemplate(int templateId)
        {
            var templateFilesNames = GetTemplateFilesNames(templateId);

            return _posScreenTemplatesFilesInfo.RequiredFilesList
                .Except(templateFilesNames, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public string GetTemplateDirectoryAbsolutePath(int templateId)
        {
            return Path.Combine(
                _posScreenTemplatesFilesInfo.WebRootPath,
                GetTemplateDirectoryPath(templateId));
        }
    }
}