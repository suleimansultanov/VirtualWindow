using System.Collections.Generic;
using NasladdinPlace.Utilities.Models;

namespace NasladdinPlace.Core.Models.Documents.Contracts
{
    public interface IDocument<T> : IDocumentHeader where T: class
    {
        int Id { get;}
        ICollection<T> TablePart { get; set; }
        bool IsPosted { get; }
        bool IsDeleted { get; }
        Result MarkAsDeleted();
        Result MarkAsPosted();
        Result MarkAsCanceled();
        void AddTablePartItems(ICollection<T> tableItems);
    }
}
