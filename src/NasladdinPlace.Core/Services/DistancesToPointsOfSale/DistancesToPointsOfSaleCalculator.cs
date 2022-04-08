using System.Collections.Generic;
using System.Collections.ObjectModel;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.DistancesToPointsOfSale.Models;
using NasladdinPlace.Core.Services.LocationsDistance;

namespace NasladdinPlace.Core.Services.DistancesToPointsOfSale
{
    public class DistancesToPointsOfSaleCalculator : IDistancesToPointsOfSaleCalculator
    {
        private readonly ILocationsDistanceCalculator _locationsDistanceCalculator;

        public DistancesToPointsOfSaleCalculator(ILocationsDistanceCalculator locationsDistanceCalculator)
        {
            _locationsDistanceCalculator = locationsDistanceCalculator;
        }

        public DistanceToPos CalculateDistance(Location from, Core.Models.Pos toPos)
        {
            var posLocation = new Location(toPos.Latitude, toPos.Longitude);
            var distanceInKm = _locationsDistanceCalculator.CalculateDistanceInKilometers(from, posLocation);
            return new DistanceToPos(toPos, distanceInKm);
        }

        public IEnumerable<DistanceToPos> CalculateDistances(Location from, IEnumerable<Core.Models.Pos> toPointOfSales)
        {
            var distancesToPointsOfSale = new Collection<DistanceToPos>();

            foreach (var plant in toPointOfSales)
            {
                distancesToPointsOfSale.Add(CalculateDistance(from, plant));
            }

            return distancesToPointsOfSale;
        }
    }
}
