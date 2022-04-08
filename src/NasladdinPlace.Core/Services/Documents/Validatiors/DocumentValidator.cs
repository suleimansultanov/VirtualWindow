using NasladdinPlace.Core.Models.Documents.Contracts;
using NasladdinPlace.Core.Services.Documents.Validatiors.Contracts;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Documents.Validatiors
{
    public class DocumentValidator<T> : IDocumentValidator<T> where T: class

    {
        public Result ValidateOnSave(IDocument<T> document)
        {
            return CheckDocumentOnAbilityToPost(document);
        }

        public Result ValidateOnDelete(IDocument<T> document)
        {
            return CheckDocumentOnNull(document);
        }

        public Result ValidateOnPost(IDocument<T> document)
        {
            return CheckDocumentOnAbilityToPost(document);
        }

        public Result ValidateOnCancel(IDocument<T> document)
        {
            return CheckDocumentOnNull(document);
        }

        private Result CheckDocumentOnNull(IDocument<T> document)
        {
            return document == null ? Result.Failure("Document can not be null") : Result.Success();
        }

        private Result CheckDocumentOnAbilityToPost(IDocument<T> document)
        {
            var checkOnNullResult = CheckDocumentOnNull(document);
            if (checkOnNullResult.Succeeded)
                return document.IsPosted ? Result.Failure("Document has been already posted") : Result.Success();

            return checkOnNullResult;
        }
    }
}
