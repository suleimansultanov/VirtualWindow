using System.Collections.Generic;
using NasladdinPlace.Core.Models;
using NasladdinPlace.Core.Services.DistancesToPointsOfSale.Models;

namespace NasladdinPlace.Core.Services.DistancesToPointsOfSale
{
    public interface IDistancesToPointsOfSaleCalculator
    {
        DistanceToPos CalculateDistance(Location from, Core.Models.Pos toPos);
        IEnumerable<DistanceToPos> CalculateDistances(Location from, IEnumerable<Core.Models.Pos> toPointOfSales);
    }
}
