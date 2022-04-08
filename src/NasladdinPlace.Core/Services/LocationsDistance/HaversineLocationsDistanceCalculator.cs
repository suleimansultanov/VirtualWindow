using System;
using NasladdinPlace.Core.Models;

namespace NasladdinPlace.Core.Services.LocationsDistance
{
    public class HaversineLocationsDistanceCalculator : ILocationsDistanceCalculator
    {
        private const double QuatorialEarthRadius = 6378.1370D;
        private const double D2R = Math.PI / 180D;

        public double CalculateDistanceInKilometers(Location first, Location second)
        {
            var dlong = (second.Longitude - first.Longitude) * D2R;
            var dlat = (second.Latitude - first.Latitude) * D2R;
            var a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(first.Latitude * D2R) * Math.Cos(second.Latitude * D2R) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            var c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            var d = QuatorialEarthRadius * c;

            return d;
        }

        public double CalculateDistanceInMeters(Location first, Location second)
        {
            return (int)(1000D * CalculateDistanceInKilometers(first, second));
        }
    }
}
