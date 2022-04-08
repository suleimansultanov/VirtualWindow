using NasladdinPlace.Core.Models.Documents.Contracts;
using NasladdinPlace.Core.Models.Documents.GoodsMoving;
using NasladdinPlace.Core.Services.Documents.Validatiors.Contracts;
using NasladdinPlace.Logging;

namespace NasladdinPlace.Core.Services.Documents.Managers
{
    public class DocumentGoodsMovingManager : BaseDocumentManager<DocumentGoodsMovingTableItem>
    {
        public DocumentGoodsMovingManager(IDocumentValidator<DocumentGoodsMovingTableItem> documentValidator, ILogger logger) : base(documentValidator, logger)
        {
        }

        protected override void ProcessOnSave(IDocument<DocumentGoodsMovingTableItem> document, IUnitOfWork unitOfWork)
        {
            unitOfWork.DocumentsGoodsMoving.Add(document as DocumentGoodsMoving);
        }
    }
}
