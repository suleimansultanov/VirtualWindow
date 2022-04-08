using System.Threading.Tasks;
using NasladdinPlace.Core.Models.Documents.Contracts;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Services.Documents.Managers.Contracts
{
    public interface IDocumentManager<T> where T: class
    {
        Task<Result> SaveAsync(IDocument<T> document, IUnitOfWork unitOfWork);
        Task<Result> DeleteAsync(IDocument<T> document, IUnitOfWork unitOfWork);
        Task<Result> PostAsync(IDocument<T> document, IUnitOfWork unitOfWork);
        Task<Result> CancelAsync(IDocument<T> document, IUnitOfWork unitOfWork);
    }
}
