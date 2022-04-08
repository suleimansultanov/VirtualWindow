using System;
using System.Threading.Tasks;
using NasladdinPlace.Core.Models.Documents.Contracts;
using NasladdinPlace.Core.Services.Documents.Managers.Contracts;
using NasladdinPlace.Core.Services.Documents.Validatiors.Contracts;
using NasladdinPlace.Logging;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Documents.Managers
{
    public abstract class BaseDocumentManager<T> : IDocumentManager<T> where T : class
    {
        private readonly IDocumentValidator<T> _documentValidator;
        private readonly ILogger _logger;

        protected BaseDocumentManager(
            IDocumentValidator<T> documentValidator,
            ILogger logger)
        {
            if (documentValidator == null)
                throw new ArgumentNullException(nameof(documentValidator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _documentValidator = documentValidator;
            _logger = logger;
        }

        public async Task<Result> SaveAsync(IDocument<T> document, IUnitOfWork unitOfWork)
        {
            var validationResult = _documentValidator.ValidateOnSave(document);
            if (!validationResult.Succeeded)
                return validationResult;

            return await ExecuteAsync(unitOfWork, () => ProcessOnSave(document, unitOfWork));
        }

        public async Task<Result> DeleteAsync(IDocument<T> document, IUnitOfWork unitOfWork)
        {
            var validationResult = _documentValidator.ValidateOnDelete(document);
            if (!validationResult.Succeeded)
                return validationResult;

            var markResult = document.MarkAsDeleted();

            if (!markResult.Succeeded)
                return markResult;

            return await ExecuteAsync(unitOfWork, () => ProcessOnDelete(document, unitOfWork));
        }

        public async Task<Result> PostAsync(IDocument<T> document, IUnitOfWork unitOfWork)
        {
            var validationResult = _documentValidator.ValidateOnPost(document);
            if (!validationResult.Succeeded)
                return validationResult;

            var markResult = document.MarkAsPosted();

            if (!markResult.Succeeded)
                return markResult;

            return await ExecuteAsync(unitOfWork, () => ProcessOnPost(document, unitOfWork));
        }

        public async Task<Result> CancelAsync(IDocument<T> document, IUnitOfWork unitOfWork)
        {
            var validationResult = _documentValidator.ValidateOnCancel(document);
            if (!validationResult.Succeeded)
                return validationResult;

            var markResult = document.MarkAsCanceled();

            if (!markResult.Succeeded)
                return markResult;

            return await ExecuteAsync(unitOfWork, () => ProcessOnCancel(document, unitOfWork));
        }

        protected virtual void ProcessOnSave(IDocument<T> document, IUnitOfWork unitOfWork) { }
        protected virtual void ProcessOnDelete(IDocument<T> document, IUnitOfWork unitOfWork) { }
        protected virtual void ProcessOnPost(IDocument<T> document, IUnitOfWork unitOfWork) { }
        protected virtual void ProcessOnCancel(IDocument<T> document, IUnitOfWork unitOfWork) { }

        private async Task<Result> ExecuteAsync(IUnitOfWork unitOfWork, Action action)
        {
            try
            {
                action?.Invoke();
                await unitOfWork.CompleteAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                var errorMessage = $"An error occurred while trying save document. Error: {ex}";
                _logger.LogError(errorMessage);

                return Result.Failure(errorMessage);
            }
        }
    }
}
