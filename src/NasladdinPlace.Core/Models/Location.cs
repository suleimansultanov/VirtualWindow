namespace NasladdinPlace.Core.Models
{
    public struct Location
    {
        public static bool TryParse(string latitude, string longitude, out Location location)
        {
            if (!double.TryParse(latitude, out var latitudeAsDouble) || !double.TryParse(longitude, out var longitudeAsDouble))
            {
                location = new Location();
                return false;
            }
            
            location = new Location(latitudeAsDouble, longitudeAsDouble);
            return true;
        }
        
        public double Latitude { get; }
        public double Longitude { get; }
        
        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
