using System;

namespace NasladdinPlace.Core.Models.Documents.Contracts
{
    public interface IDocumentHeader
    {
        Guid? ErpId { get; }
        string DocumentNumber { get; set; }
        DateTime CreatedDate { get; }
        DateTime? ModifiedDate { get; }
    }
}
