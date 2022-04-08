using NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Contracts;

namespace NasladdinPlace.Core.Services.ActivityManagement.PosComponents
{
    public class PosComponentsActivityManager : IPosComponentsActivityManager
    {
        public IActivityManager<int> PosDisplays { get; }
        public IActivityManager<int> PointsOfSale { get; }
        public IActivityManager<int> PosBattery { get; }

        public PosComponentsActivityManager(IActivityManager<int> posDisplays,
            IActivityManager<int> pointsOfSale,
            IActivityManager<int> posBattery)
        {
            PosDisplays = posDisplays;
            PointsOfSale = pointsOfSale;
            PosBattery = posBattery;
        }
    }
}