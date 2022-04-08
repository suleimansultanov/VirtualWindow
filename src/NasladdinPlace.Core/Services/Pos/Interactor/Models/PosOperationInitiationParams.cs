using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Services.Pos.Interactor.Models
{
    public class PosOperationInitiationParams
    {
        public static PosOperationInitiationParams ForPurchase(int userId, int posId, Brand brand, PosDoorPosition? doorPosition = null)
        {
            return new PosOperationInitiationParams(userId, posId)
            {
                Brand = brand,
                PosMode = PosMode.Purchase,
                DoorPosition = doorPosition ?? PosDoorPosition.Left
            };
        }

        public static PosOperationInitiationParams ForGoodsPlacing(int userId, int posId, PosDoorPosition doorPosition)
        {
            return new PosOperationInitiationParams(userId, posId)
            {
                DoorPosition = doorPosition,
                PosMode = PosMode.GoodsPlacing
            };
        }
        
        public static PosOperationInitiationParams ForGoodsIdentification(int userId, int posId, PosDoorPosition doorPosition)
        {
            return new PosOperationInitiationParams(userId, posId)
            {
                DoorPosition = doorPosition,
                PosMode = PosMode.GoodsIdentification
            };
        }
        
        public int UserId { get; }
        public int PosId { get; }
        public PosDoorPosition DoorPosition { get; set; }
        public Brand Brand { get; set; }
        public PosMode PosMode { get; set; }

        private PosOperationInitiationParams(int userId, int posId)
        {
            UserId = userId;
            PosId = posId;
            DoorPosition = PosDoorPosition.Left;
            Brand = Brand.Invalid;
            PosMode = PosMode.Purchase;
        }
    }
}