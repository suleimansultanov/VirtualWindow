using System;

namespace NasladdinPlace.Core.Models
{
    public class City : Entity, ICommonHandbook
    {
        public Country Country { get; private set; }

        public int CountryId { get; private set; }
        public string Name { get; private set; }

        protected City()
        {
            // intentionally left empty
        }
        
        public City(int id, string name, int countryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            
            Id = id;
            Name = name;
            CountryId = countryId;
        }
    }
}
