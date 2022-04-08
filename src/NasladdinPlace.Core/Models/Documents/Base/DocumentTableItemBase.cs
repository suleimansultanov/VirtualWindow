using NasladdinPlace.Core.Models.Documents.Contracts;

namespace NasladdinPlace.Core.Models.Documents.Base
{
    public abstract class DocumentTableItemBase<T> : Entity, IDocumentTableItem<T> where T : class
    {
        public T Document { get; set; }
        public int DocumentId { get; set; }
        public int LineNum { get; set; }
    }
}
