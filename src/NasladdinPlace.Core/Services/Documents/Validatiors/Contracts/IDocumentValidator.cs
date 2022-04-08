using NasladdinPlace.Core.Models.Documents.Contracts;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Documents.Validatiors.Contracts
{
    public interface IDocumentValidator<T> where T: class
    {
        Result ValidateOnSave(IDocument<T> document);
        Result ValidateOnDelete(IDocument<T> document);
        Result ValidateOnPost(IDocument<T> document);
        Result ValidateOnCancel(IDocument<T> document);
    }
}
