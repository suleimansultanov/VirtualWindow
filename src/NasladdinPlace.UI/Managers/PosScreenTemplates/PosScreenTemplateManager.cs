using Microsoft.AspNetCore.Http;
using NasladdinPlace.Core;
using NasladdinPlace.Core.Models;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Files.Contracts;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Validators.Contracts;
using NasladdinPlace.UI.ViewModels.PointsOfSale;
using NasladdinPlace.Utilities.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace NasladdinPlace.UI.Managers.PosScreenTemplates
{
    public class PosScreenTemplateManager : IPosScreenTemplateManager
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IPosScreenTemplateFilesManager _posScreenTemplateFilesManager;
        private readonly IPosScreenTemplateValidator _posScreenTemplateValidator;
        private readonly int _defaultPosScreenTemplateId;

        public PosScreenTemplateManager(
            IUnitOfWorkFactory unitOfWorkFactory,
            IPosScreenTemplateFilesManager posScreenTemplateFilesManager,
            IPosScreenTemplateValidator posScreenTemplateValidator,
            int defaultPosScreenTemplateId)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (posScreenTemplateFilesManager == null)
                throw new ArgumentNullException(nameof(posScreenTemplateFilesManager));
            if (posScreenTemplateValidator == null)
                throw new ArgumentNullException(nameof(posScreenTemplateValidator));

            _unitOfWorkFactory = unitOfWorkFactory;
            _posScreenTemplateFilesManager = posScreenTemplateFilesManager;
            _posScreenTemplateValidator = posScreenTemplateValidator;
            _defaultPosScreenTemplateId = defaultPosScreenTemplateId;
        }

        public async Task<Result> CreatePosScreenTemplateAsync(string posScreenTemplateName)
        {
            var validationResults = await _posScreenTemplateValidator.ValidateWhetherExistsAsync(posScreenTemplateName);

            if (!CheckValidationResults(validationResults, out var validationError))
                return Result.Failure(validationError);

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var template = new PosScreenTemplate(posScreenTemplateName);

                unitOfWork.PosScreenTemplates.Add(template);

                await unitOfWork.CompleteAsync();

                var templateCreationResult = await _posScreenTemplateFilesManager.CreateTemplateDirectoryAsync(template.Id);

                if (templateCreationResult.Succeeded)
                    return Result.Success();

                unitOfWork.PosScreenTemplates.Remove(template.Id);
                await unitOfWork.CompleteAsync();

                return templateCreationResult;
            }
        }

        public async Task<Result> EditPosScreenTemplateAsync(int posScreenTemplateId, string posScreenTemplateName, IEnumerable<PosBasicInfoViewModel> pointsOfSale)
        {
            var validationResults =
                await _posScreenTemplateValidator.ValidateWhetherExistsAsync(posScreenTemplateId,
                    posScreenTemplateName);

            if (!CheckValidationResults(validationResults, out var validationError))
                return Result.Failure(validationError);

            var requiredFiles = _posScreenTemplateFilesManager.GetMissingRequiredFileNamesForTemplate(posScreenTemplateId);
            if (requiredFiles.Any())
                return Result.Failure(
                    $"В перечне файлов шаблона не найдено обязательных файлов : {string.Join(", ", requiredFiles)}");

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var template =
                    await unitOfWork.PosScreenTemplates.GetIncludingPointsOfSaleAsync(posScreenTemplateId);

                template.UnlinkPointsOfSale(_defaultPosScreenTemplateId);

                var pointsOfSaleIds = pointsOfSale.Select(pos => pos.PosId).ToImmutableHashSet();
                var selectedPointsOfSaleByIds = await unitOfWork.PointsOfSale.GetByIdsAsync(pointsOfSaleIds);

                foreach (var pos in selectedPointsOfSaleByIds)
                {
                    pos.UpdatePosScreenTemplate(template.Id);
                }

                template.Name = posScreenTemplateName;

                await unitOfWork.CompleteAsync();

                return Result.Success();
            }
        }

        public async Task<Result> DeletePosScreenTemplateAsync(int posScreenTemplateId)
        {
            var validationResults = _posScreenTemplateValidator.ValidateWhetherDefault(posScreenTemplateId);

            if (!CheckValidationResults(validationResults, out var validationError))
                return Result.Failure(validationError);

            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                var template = await unitOfWork.PosScreenTemplates.GetIncludingPointsOfSaleAsync(posScreenTemplateId);
                if (template.HasLinkedPointsOfSale)
                    return Result.Failure();

                unitOfWork.PosScreenTemplates.Remove(template.Id);

                var deletionResult = await _posScreenTemplateFilesManager.DeleteTemplateDirectoryAsync(template.Id);

                if(!deletionResult.Succeeded)
                    return deletionResult;

                await unitOfWork.CompleteAsync();

                return Result.Success();
            }
        }

        public async Task<Result> UploadTemplateFileAsync(int posScreenTemplateId, IFormFile file)
        {
            var validationResults = _posScreenTemplateValidator.ValidateWhetherExists(posScreenTemplateId);

            if (!CheckValidationResults(validationResults, out var validationError))
                return Result.Failure(validationError);

            if (file == null || file.Length == 0)
                return Result.Failure("Загружаемый файл не существует");

            return await _posScreenTemplateFilesManager.UploadTemplateFileAsync(posScreenTemplateId, file);
        }

        public async Task<Result> DeleteTemplateFileAsync(int posScreenTemplateId, string fileName)
        {
            var validationResults = _posScreenTemplateValidator.ValidateWhetherExists(posScreenTemplateId);

            if (!CheckValidationResults(validationResults, out var validationError))
                return Result.Failure(validationError);

            return await _posScreenTemplateFilesManager.DeleteTemplateFileAsync(posScreenTemplateId, fileName);
        }

        private bool CheckValidationResults(IEnumerable<Result> validationResults, out string message)
        {
            message = string.Empty;
            if (validationResults.All(vr => vr.Succeeded))
                return true;

            message = string.Join(". ", validationResults.Select(vr => vr.Error));
            return false;
        }
    }
}