using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NasladdinPlace.Core.Models
{
    public class Country : Entity, ICommonHandbook
    {
        public ICollection<City> Cities { get; private set; }

        public string Name { get; private set; }

        protected Country()
        {
            // intentionally left empty
        }
        
        public Country(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            
            Id = id;
            Name = name;
            Cities = new Collection<City>();
        }
    }
}
