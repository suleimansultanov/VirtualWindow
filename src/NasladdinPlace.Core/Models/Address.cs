using System;

namespace NasladdinPlace.Core.Models
{
    public class Address
    {
        public static Address FromCityStreetAtCoordinates(int cityId, string street, Location location, string accurateLocation = null)
        {
            return new Address(cityId, street, location, accurateLocation);
        }
        
        public Location Location { get; }
        public string Street { get; }
        public int CityId { get; }
        public string AccurateLocation { get; }

        private Address(int cityId, string street, Location location, string accurateLocation)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentNullException(nameof(street));
            
            CityId = cityId;
            Street = street;
            Location = location;
            AccurateLocation = accurateLocation;
        }
    }
}