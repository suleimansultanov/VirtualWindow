using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NasladdinPlace.Core;
using NasladdinPlace.UI.Managers.PosScreenTemplates.Validators.Contracts;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.UI.Managers.PosScreenTemplates.Validators
{
    public class PosScreenTemplateValidator : IPosScreenTemplateValidator
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly int _defaultPosScreenTemplateId;

        public PosScreenTemplateValidator(IUnitOfWorkFactory unitOfWorkFactory, int defaultPosScreenTemplateId)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentException(nameof(unitOfWorkFactory));

            _unitOfWorkFactory = unitOfWorkFactory;
            _defaultPosScreenTemplateId = defaultPosScreenTemplateId;
        }

        public async Task<IReadOnlyCollection<Result>> ValidateWhetherExistsAsync(string posScreenTemplateName)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return new List<Result>
                {
                    await CheckWhetherTemplateWithNameExistsAsync(unitOfWork, posScreenTemplateName)
                };
            }
        }

        public async Task<IReadOnlyCollection<Result>> ValidateWhetherExistsAsync(int posScreenTemplateId, string posScreenTemplateName)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return new List<Result>
                {
                    CheckWhetherTemplateWithIdExists(unitOfWork, posScreenTemplateId),
                    await CheckWhetherTemplateExistsAsync(unitOfWork, posScreenTemplateId, posScreenTemplateName)
                };
            }
        }

        public IReadOnlyCollection<Result> ValidateWhetherDefault(int posScreenTemplateId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return new List<Result>
                {
                    CheckWhetherTemplateWithIdExists(unitOfWork, posScreenTemplateId),
                    CheckWhetherTemplateIsDefault(posScreenTemplateId)
                };
            }
        }

        public IReadOnlyCollection<Result> ValidateWhetherExists(int posScreenTemplateId)
        {
            using (var unitOfWork = _unitOfWorkFactory.MakeUnitOfWork())
            {
                return new List<Result>
                {
                    CheckWhetherTemplateWithIdExists(unitOfWork, posScreenTemplateId)
                };
            }
        }

        private async Task<Result> CheckWhetherTemplateWithNameExistsAsync(IUnitOfWork unitOfWork, string templateName)
        {
            var existingTemplateByName = await unitOfWork.PosScreenTemplates.GetByNameAsync(templateName);
            return existingTemplateByName != null
                ? Result.Failure($"Шаблон с наименованием '{templateName}' уже существует")
                : Result.Success();
        }

        private async Task<Result> CheckWhetherTemplateExistsAsync(IUnitOfWork unitOfWork, int templateId, string templateName)
        {
            var existingTemplateByName = await unitOfWork.PosScreenTemplates.GetByNameAsync(templateName);
            if (existingTemplateByName == null || existingTemplateByName.Id == templateId)
                return Result.Success();

            return Result.Failure($"Шаблон с наименованием '{templateName}' уже существует");
        }

        private Result CheckWhetherTemplateWithIdExists(IUnitOfWork unitOfWork, int templateId)
        {
            var existingTemplateById = unitOfWork.PosScreenTemplates.GetById(templateId);
            return existingTemplateById == null
                ? Result.Failure($"Не найден шаблон по указанному идентификатору '{templateId}'")
                : Result.Success();
        }

        private Result CheckWhetherTemplateIsDefault(int templateId)
        {
            return templateId == _defaultPosScreenTemplateId
                ? Result.Failure($"Невозможно удалить шаблон, который установлен как шаблон по умолчанию")
                : Result.Success();
        }
    }
}