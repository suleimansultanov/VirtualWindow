using System;

namespace NasladdinPlace.Core.Services.DistancesToPointsOfSale.Models
{
    public class DistanceToPos
    {
        public Core.Models.Pos Pos { get; }
        public double DistanceInKm { get; }
        public double RoundedDistanceInKm { get; }

        public DistanceToPos(Core.Models.Pos pos, double distanceInKm)
        {
            if (pos == null)
                throw new ArgumentNullException(nameof(pos));
            if (distanceInKm < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(distanceInKm),
                    distanceInKm, 
                    "The distance to keep cannot be less than zero."
                );

            Pos = pos;
            DistanceInKm = distanceInKm;
            RoundedDistanceInKm = Math.Round(DistanceInKm, 3);
        }
    }
}
