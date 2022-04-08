namespace NasladdinPlace.Core.Services.ActivityManagement.PosComponents.Contracts
{
    public interface IPosComponentsActivityManager
    {
        IActivityManager<int> PosDisplays { get; }
        IActivityManager<int> PointsOfSale { get; }
        IActivityManager<int> PosBattery { get; }
    }
}