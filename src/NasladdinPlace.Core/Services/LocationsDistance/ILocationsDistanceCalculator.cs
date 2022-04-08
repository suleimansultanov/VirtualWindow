using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.LocationsDistance
{
    public interface ILocationsDistanceCalculator
    {
        double CalculateDistanceInKilometers(Location first, Location second);
        double CalculateDistanceInMeters(Location first, Location second);
    }
}
