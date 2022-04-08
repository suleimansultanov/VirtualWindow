namespace NasladdinPlace.Core.Models.Documents.Contracts
{
    public interface IDocumentTableItem<T> where T : class
    {
        int Id { get; }
        T Document { get; set; }
        int DocumentId { get; set; }
        int LineNum { get; }
    }
}
