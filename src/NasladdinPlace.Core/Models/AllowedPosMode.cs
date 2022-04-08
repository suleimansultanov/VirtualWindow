using NasladdinPlace.Core.Enums;

namespace NasladdinPlace.Core.Models
{
    public class AllowedPosMode
    {
        public int PosId { get; protected set; }
        public PosMode Mode { get; protected set; }

        protected AllowedPosMode()
        {
            // required for EF
        }
        
        public AllowedPosMode(int posId, PosMode mode)
        {
            PosId = posId;
            Mode = mode;
        }
    }
}