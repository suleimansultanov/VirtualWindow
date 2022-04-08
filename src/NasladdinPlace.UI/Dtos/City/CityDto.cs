using NasladdinPlace.UI.Dtos.Country;
using NasladdinPlace.UI.Dtos.Shared;

namespace NasladdinPlace.UI.Dtos.City
{
    public class CityDto : ICommonHandbook
    {
        public CountryDto Country { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
