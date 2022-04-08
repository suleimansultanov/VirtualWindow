using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.Pos.OperationsManager
{
    public class OperationCreationParams
    {
        public int UserId { get; }
        public int PosId { get; }
        public Brand Brand { get; set; }
        public PosMode PosMode { get; set; }

        public OperationCreationParams(int userId, int posId)
        {
            UserId = userId;
            PosId = posId;
            Brand = Brand.Invalid;
            PosMode = PosMode.Purchase;
        }
    }
}